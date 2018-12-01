using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BasicEnemyController))]
public class BasicEnemyEditor : Editor
{
    BasicEnemyController t
    {
        get { return (BasicEnemyController)target; }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Snap folow_origin"))
        {
            t.folow_origin = t.transform.position;
        }
    }

    private void OnSceneGUI()
    {
        if(t.target != null)
        {
            Vector3 target_position  = t.target.transform.position;
            Vector3 current_position = t.transform.position;
            Vector3 folow_origin     = t.folow_origin;

            Handles.DrawDottedLine(
                target_position,
                folow_origin,
                2
            );

            Vector3 middle = (target_position + folow_origin) * 0.5f;
            float diameter = Vector3.Distance(target_position, folow_origin);
            float radius = diameter * 0.5f;
            Handles.DrawWireDisc(middle, Vector3.up, radius);

            Vector3 direction = (target_position - folow_origin).normalized;

            Vector3 projection = Vector3.Project(
                current_position - folow_origin,
                direction
                ) + folow_origin;

            float dot = Vector3.Dot(
                current_position - folow_origin,
                direction
            );


            float dot_perpendicular = Vector3.Dot(
                current_position - folow_origin,
                direction
            );
            //dot /= diameter;

            Vector3 pos = current_position;
            const int STEP = 16;
            //float length = diameter - dot;
            //float step_length = length / STEP;
            Vector3 direction_clockwise_perpendicular = Quaternion.Euler(0,90,0) * direction;
            for (int i = 0; i < STEP; ++i)
            {
                float length = Mathf.MoveTowards(dot/diameter , 1.0f, i * 0.25f);
                float p = Mathf.Sqrt(1-Mathf.Pow((1-(length)*2), 2));
                Vector3 new_pos =
                    folow_origin + direction * length * diameter +
                    direction_clockwise_perpendicular * p * radius * 0.5f;
                Handles.DrawLine(pos, new_pos);
                pos = new_pos;
            }

            pos = folow_origin;
            for (int i = 0; i < STEP; ++i)
            {
                float length = (float)i/(float)STEP;
                float p = Mathf.Sqrt(1-Mathf.Pow((1-(length)*2), 2));
                Vector3 new_pos =
                    folow_origin + direction * length * diameter +
                    direction_clockwise_perpendicular * p * radius * 0.5f;
                Handles.DrawLine(pos, new_pos);
                pos = new_pos;
            }

            Handles.DrawDottedLine(
                current_position,
                projection,
                2
            );
        }
    }

}
