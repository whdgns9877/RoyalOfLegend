using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Photon.Pun;
using Photon.Realtime;

public delegate void TargetsVisibilityChange(List<Transform> newTargets);

[ExecuteInEditMode]
public class FieldOfView : MonoBehaviour
{
    public PhotonView PV;

    public float viewRadius;

    public GameObject me;

    [Range(0, 360)]
    public float viewAngle;

    public float viewDepth;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    //[HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();

    public int meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;


    public MeshFilter viewMeshFilter;
    public bool debug;
    Mesh viewMesh;

    public event TargetsVisibilityChange OnTargetsVisibilityChange;

    public FogProjector fogProjector;
    public float updateDistance = 1;
    Vector3 lastUpdatePos;

    GameObject searchPlayer;
    private bool compareTarget = true;

    void OnEnable()
    {
        searchPlayer = GameObject.FindGameObjectWithTag("Player");

        viewMesh = new Mesh { name = "View Mesh" };
        viewMeshFilter.mesh = viewMesh;

        fogProjector = fogProjector ?? FindObjectOfType<FogProjector>();

        StartCoroutine("FindTargetsWithDelay", 2f);
        StartCoroutine("FindTargets", 2f);
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    IEnumerator FindTargets(float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("me Layer은? " + me.layer);
        //Debug.Log("Before target Layer은? " + targetMask.value);
        if (me.layer == 13) //BlueHero
            targetMask = 1 << 15;
        if (me.layer == 15)
            targetMask = 1 << 13;
        //Debug.Log("After target Layer은? " + targetMask.value);
    }

    void LateUpdate()
    {
        if (searchPlayer.layer == LayerMask.NameToLayer("BlueHero"))
        {
            if (gameObject.transform.parent.gameObject.layer == LayerMask.NameToLayer("BlueHero"))
            {
                DrawFieldOfView();
                if (Vector3.Distance(transform.position, lastUpdatePos) > updateDistance || Time.time < .5f)
                {
                    lastUpdatePos = transform.position;
                    fogProjector.UpdateFog();
                }
            }
        }

        if (searchPlayer.layer == LayerMask.NameToLayer("RedHero"))
        {
            if (gameObject.transform.parent.gameObject.layer == LayerMask.NameToLayer("RedHero"))
            {
                DrawFieldOfView();
                if (Vector3.Distance(transform.position, lastUpdatePos) > updateDistance || Time.time < .5f)
                {
                    lastUpdatePos = transform.position;
                    fogProjector.UpdateFog();
                }
            }
        }
    }

    void FindVisibleTargets()
    {
        if (PV.IsMine)
        {
            GameObject[] teamHero = GameObject.FindGameObjectsWithTag("TeamHero");

            visibleTargets.Clear();
            for (int a = 0; a < teamHero.Length; a++)
            {
                if (teamHero[a].transform.childCount == 11)
                {
                    for (int i = 2; i <= 4; i++)
                    {
                        teamHero[a].transform.GetChild(10).GetComponent<FieldOfView>().visibleTargets.Clear();
                    }
                }
                if (teamHero[a].transform.childCount == 7)
                {
                    teamHero[a].transform.GetChild(6).GetComponent<FieldOfView>().visibleTargets.Clear();
                }
            }

            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
            Debug.Log(targetsInViewRadius.ToString() + "targetsInViewRadius배열");

            if(teamHero.Length > 0) AddViewTarget_Team(teamHero);

            Debug.Log(targetsInViewRadius + "targetsInViewRadius????");

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                //if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                //{
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    visibleTargets.Add(target);
                    foreach (var item in visibleTargets)
                    {
                        Debug.Log(item.ToString() + " visibletarget_ME");
                    }
                }
                //}
            }

            if (OnTargetsVisibilityChange != null) OnTargetsVisibilityChange(visibleTargets);
        }
    }

    void AddViewTarget_Team(GameObject[] teamHero)
    {
        for(int j = 0; j < teamHero.Length; j++)
        {
            Collider[] team_targetsInViewRadius = Physics.OverlapSphere(teamHero[j].transform.position, viewRadius, targetMask);

            for (int i = 0; i < team_targetsInViewRadius.Length; i++)
            {
                Transform target = team_targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;

                float dstToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask)) visibleTargets.Add(target);
            }
        }
    }

    void DrawFieldOfView()
    {
        float stepAngleSize = viewAngle / meshResolution;
        List<Vector3> viewPoints = new List<Vector3>();
        ObstacleInfo oldObstacle = new ObstacleInfo();
        for (int i = 0; i <= meshResolution; i++)
        {
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
            ObstacleInfo newObstacle = FindObstacles(angle);

            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldObstacle.dst - newObstacle.dst) > edgeDstThreshold;
                if (oldObstacle.hit != newObstacle.hit ||
                    oldObstacle.hit && edgeDstThresholdExceeded)
                {
                    EdgeInfo edge = FindEdge(oldObstacle, newObstacle);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            viewPoints.Add(newObstacle.point);
            oldObstacle = newObstacle;
        }

        int vertexCount = viewPoints.Count + 1;
        var vertices = new Vector3[vertexCount];
        var triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        viewMesh.Clear();

        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }


    EdgeInfo FindEdge(ObstacleInfo minObstacle, ObstacleInfo maxObstacle)
    {
        float minAngle = minObstacle.angle;
        float maxAngle = maxObstacle.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ObstacleInfo newObstacle = FindObstacles(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minObstacle.dst - newObstacle.dst) > edgeDstThreshold;
            if (newObstacle.hit == minObstacle.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newObstacle.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newObstacle.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    ObstacleInfo FindObstacles(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (DebugRayCast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ObstacleInfo(true, hit.point + hit.normal * -viewDepth, hit.distance, globalAngle);
        }
        return new ObstacleInfo(false, transform.position + dir * (viewRadius - viewDepth), viewRadius, globalAngle);
    }

    bool DebugRayCast(Vector3 origin, Vector3 direction, out RaycastHit hit, float maxDistance, int mask)
    {
        if (Physics.Raycast(origin, direction, out hit, maxDistance, mask))
        {
            if (debug)
                Debug.DrawLine(origin, hit.point);
            return true;
        }
        if (debug)
            Debug.DrawLine(origin, origin + direction * maxDistance);
        return false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool isGlobal)
    {
        if (!isGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ObstacleInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ObstacleInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}