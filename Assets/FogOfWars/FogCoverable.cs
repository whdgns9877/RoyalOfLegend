using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class FogCoverable : MonoBehaviour
{
    Renderer renderer;

    public PhotonView PV;

    //private event TargetsVisibilityChange OnTargetsVisibilityChange;
    private List<Transform> meVisibleTargets_FogCoverable = new List<Transform>();
    private List<Transform>[] team_VisibleTargets_FogCoverable;

    public bool meActive = true;

    private bool compareTarget = true;

    void Start()
    {
        renderer = GetComponent<Renderer>();

        StartCoroutine(SearchFieldOfView(5));
    }

    //void OnDestroy()
    //{
    //    GameObject mePlayer = GameObject.FindGameObjectWithTag("Player");
    //    GameObject[] teamHero = GameObject.FindGameObjectsWithTag("TeamHero");

    //    meVisibleTargets_FogCoverable = mePlayer.transform.GetChild(9).GetComponent<FieldOfView>().visibleTargets;
    //    Debug.Log(meVisibleTargets_FogCoverable + " meVisibleTargets_FogCoverable임");

    //    for (int i = 0; i < teamHero.Length; i++)
    //    {
    //        team_VisibleTargets_FogCoverable[i] = teamHero[i].transform.GetChild(9).GetComponent<FieldOfView>().visibleTargets;
    //        Debug.Log(team_VisibleTargets_FogCoverable[i] + " team_VisibleTargets_FogCoverable[" +i+"]");
    //    }

    //    //if (OnTargetsVisibilityChange != null) OnTargetsVisibilityChange(meVisibleTargets_FogCoverable);

    //    if (mePlayer.GetComponent<CharacterControl>().PV.IsMine)
    //    {
    //        mePlayer.transform.GetChild(9).GetComponent<FieldOfView>().OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;

    //        //meVisibleTargets_FogCoverable.Remove(gameObject.transform);
    //        //OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;
    //        //if (OnTargetsVisibilityChange != null) OnTargetsVisibilityChange(meVisibleTargets_FogCoverable);

    //        for (int i = 0; i < teamHero.Length; i++)
    //        {
    //            CompareTarget(mePlayer, teamHero[i]);
    //        }

    //        //for (int i = 0; i < teamHero.Length; i++)
    //        //{
    //        //    CompareTarget(meVisibleTargets_FogCoverable, team_VisibleTargets_FogCoverable[i]);
    //        //}

    //        if (compareTarget)
    //        {
    //            if (mePlayer.layer == LayerMask.NameToLayer("RedHero"))
    //            {
    //                if (gameObject.layer == LayerMask.NameToLayer("BlueHero"))
    //                {
    //                    //mePlayer.transform.GetChild(9).GetComponent<FieldOfView>().OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;
    //                    for (int i = 0; i < teamHero.Length; i++)
    //                    {
    //                        teamHero[i].transform.GetChild(9).GetComponent<FieldOfView>().OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;
    //                    }
    //                }
    //            }

    //            if (mePlayer.layer == LayerMask.NameToLayer("BlueHero"))
    //            {
    //                if (gameObject.layer == LayerMask.NameToLayer("RedHero"))
    //                {
    //                    //mePlayer.transform.GetChild(9).GetComponent<FieldOfView>().OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;
    //                    for (int i = 0; i < teamHero.Length; i++)
    //                    {
    //                        teamHero[i].transform.GetChild(9).GetComponent<FieldOfView>().OnTargetsVisibilityChange -= FieldOfViewOnTargetsVisibilityChange;
    //                    }
    //                }
    //            }
    //        }
    //        else
    //        {
    //            mePlayer.transform.GetChild(9).GetComponent<FieldOfView>().OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange; //ddsafadsf
    //        }

    //    }
    //}

    //void CompareTarget(GameObject Player, GameObject TeamPlayer)
    //{
    //    if (Player.transform.GetChild(9).GetComponent<FieldOfView>().visibleTargets != TeamPlayer.transform.GetChild(9).GetComponent<FieldOfView>().visibleTargets)
    //    {
    //        compareTarget = false;
    //    }
    //}

    //void CompareTarget(List<Transform> Player, List<Transform> TeamPlayer)
    //{
    //    if (Player != TeamPlayer)
    //    {
    //        compareTarget = false;
    //    }
    //}

    void FieldOfViewOnTargetsVisibilityChange(List<Transform> newTargets)
    {
        renderer.enabled = newTargets.Contains(transform);
        if (newTargets.Contains(transform))
            meActive = true;
        else
            meActive = false;
    }

    IEnumerator SearchFieldOfView(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        GameObject mePlayer = GameObject.FindGameObjectWithTag("Player");
        GameObject[] teamHero = GameObject.FindGameObjectsWithTag("TeamHero");

        if (mePlayer.layer == LayerMask.NameToLayer("RedHero"))
        {
            if (gameObject.layer == LayerMask.NameToLayer("BlueHero"))
            {
                if (mePlayer.transform.childCount == 11)
                    mePlayer.transform.GetChild(10).GetComponent<FieldOfView>().OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;

                if (mePlayer.transform.childCount == 7)
                    mePlayer.transform.GetChild(6).GetComponent<FieldOfView>().OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;

                for (int i = 0; i < teamHero.Length; i++)
                {
                    if(teamHero[i].transform.childCount == 11)
                        teamHero[i].transform.GetChild(10).GetComponent<FieldOfView>().OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;
                    if (teamHero[i].transform.childCount == 7)
                        teamHero[i].transform.GetChild(6).GetComponent<FieldOfView>().OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;
                }
            }
        }

        if (mePlayer.layer == LayerMask.NameToLayer("BlueHero"))
        {
            if (gameObject.layer == LayerMask.NameToLayer("RedHero"))
            {
                if (mePlayer.transform.childCount == 11)
                    mePlayer.transform.GetChild(10).GetComponent<FieldOfView>().OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;

                if (mePlayer.transform.childCount == 7)
                    mePlayer.transform.GetChild(6).GetComponent<FieldOfView>().OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;

                for (int i = 0; i < teamHero.Length; i++)
                {
                    if (teamHero[i].transform.childCount == 11)
                        teamHero[i].transform.GetChild(10).GetComponent<FieldOfView>().OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;
                    if (teamHero[i].transform.childCount == 7)
                        teamHero[i].transform.GetChild(6).GetComponent<FieldOfView>().OnTargetsVisibilityChange += FieldOfViewOnTargetsVisibilityChange;
                }
            }
        }
    }
}