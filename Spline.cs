using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Splines;

public class Spline : MonoBehaviour
{
    public enum enumTurnDirection
    {
        left,
        right,
        straight
    }
    public class Evaluation
    {
        public float t;
        public Vector3 position;
        public Vector3 tangent;
        public Vector3 acceleration; //rate of change

        public Vector3 left;
        public Vector3 right;

        public Vector3 cross;
        public enumTurnDirection turn;
    }

    public SplineContainer splinecontainer = null;
    public float precision = 0.005f;
    public List<Evaluation> points = new List<Evaluation>();

    private void Update()
    {
        Evaluate();
    }
    private void OnDrawGizmos()
    {
        foreach(Evaluation point in points)
        {
            switch (point.turn)
            {
                case enumTurnDirection.left:
                    Gizmos.color = Color.blue;
                    Gizmos.DrawWireSphere(point.position, 0.50f);
                    Gizmos.DrawLine(point.position, point.position + (point.left * 0.75f));
                    break;
                case enumTurnDirection.right:
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(point.position, 0.50f);
                    Gizmos.DrawLine(point.position, point.position + (point.right * 0.75f));  
                    break;
                case enumTurnDirection.straight:
                    Gizmos.color = Color.white;
                    Gizmos.DrawWireSphere(point.position, 0.50f);
                    break;
            }
            

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(point.position, point.position + Vector3.up * (point.acceleration.magnitude * 0.20f));

            
        }
    }
    private void Evaluate()
    {
        float delta = precision;

        for (float t = 0.0f; t < 1.0f; t += precision)
        {
            Evaluation point = new Evaluation();

            point.position = splinecontainer.EvaluatePosition(t);
            point.acceleration = splinecontainer.EvaluateAcceleration(t);

            point.tangent = splinecontainer.EvaluateTangent(t);
            point.left = new Vector3(-point.tangent.z, point.tangent.y, point.tangent.x);
            point.right = new Vector3(point.tangent.z, point.tangent.y, -point.tangent.x);

            float tfwd = t + delta;
            float tback = t - delta;
            Vector3 tangentfwd = splinecontainer.EvaluateTangent(tfwd);
            Vector3 tangentback = splinecontainer.EvaluateTangent(tback);
            point.cross = Vector3.Cross(tangentfwd, tangentback);
            if (point.cross.y > 0)
                point.turn = enumTurnDirection.left;
            else if (point.cross.y < 0)
                point.turn = enumTurnDirection.right;
            else
                point.turn = enumTurnDirection.straight;

            

            points.Add(point);
        }
    }
}
