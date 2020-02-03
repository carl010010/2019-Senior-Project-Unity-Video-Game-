using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace Utils
{
    public static class DebugUtilities
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        public static void DrawCircle(Vector3 position, float raduis)
        {
            float theta = 0;
            float x = raduis * Mathf.Cos(theta);
            float y = raduis * Mathf.Sin(theta);
            Vector3 pos = position + new Vector3(x, 0, y);
            Vector3 newPos = pos;
            Vector3 lastPos = pos;
            for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
            {
                x = raduis * Mathf.Cos(theta);
                y = raduis * Mathf.Sin(theta);
                newPos = position + new Vector3(x, 0, y);
                Gizmos.DrawLine(pos, newPos);
                pos = newPos;
            }
            Gizmos.DrawLine(pos, lastPos);
        }
    }


}
