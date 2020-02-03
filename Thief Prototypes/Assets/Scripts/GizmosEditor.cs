using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;

#if UNITY_EDITOR
namespace GizmosEditors
{
    public class GizmosEditor : EditorWindow
    {
        //All current functions to be ran by the gimo drawer class
        public static List<Action> actions = new List<Action>();

        //A gizmo Composite per object
        public static List<GizmoComposite> gizmoComposites = new List<GizmoComposite>();



        [MenuItem("Test/GizmosEditor")]
        static public void OpenWindow()
        {
            GizmosEditor gizmosEditor = GetWindow<GizmosEditor>();
            gizmosEditor.Show();
        }

        GameObject gameObject;

        private void OnEnable()
        {
            UpdateGizmoComposites();

            if (FindObjectsOfType<GizmosDrawer>().Length == 0)
            {
                gameObject = new GameObject("GizmoDrawer");
                gameObject.AddComponent<GizmosDrawer>();
            }
        }

        public static void UpdateGizmoComposites()
        {
            actions.Clear();
            gizmoComposites.Clear();

            MonoBehaviour[] sceneActive = FindObjectsOfType<MonoBehaviour>();


            foreach (MonoBehaviour mono in sceneActive)
            {
                MethodInfo[] methodFields = mono.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
                //List<Action> tempActions = new List<Action>();
                for (int i = 0; i < methodFields.Length; i++)
                {
                    GizmoMethod attribute = Attribute.GetCustomAttribute(methodFields[i], typeof(GizmoMethod)) as GizmoMethod;
                    if (attribute != null)
                    {
                        AddToOnDrawGizmos(mono.name, mono.gameObject.GetInstanceID(), attribute.groupID, (Action)Delegate.CreateDelegate(typeof(Action), mono, methodFields[i]));
                        //tempActions.Add((Action)Delegate.CreateDelegate(typeof(Action), mono, methodFields[i]));
                    }
                }
                //if (tempActions.Count > 0)
                //{
                //    AddToOnDrawGizmos(mono.name, mono.gameObject.GetInstanceID(), tempActions.ToArray());
                //    tempActions.Clear();
                //}
            }
        }

        private void OnDisable()
        {
            gizmoComposites.Clear();
        }

        Vector2 ScrollPos;

        bool dirty = false;
        //Layout the buttons to be select which function to call
        void OnGUI()
        {
            if (dirty)
            {
                UpdateGizmoComposites();
                dirty = false;
                return;
            }

            if (GUILayout.Button("Refresh"))
            {
                UpdateGizmoComposites();
                SceneView.RepaintAll();
                return;
            }

            ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);

            //For each object
            foreach (GizmoComposite g in gizmoComposites)
            {
                //EditorGUILayout.LabelField(g.objectName);
                g.toggleFoldoutControl = EditorGUILayout.Foldout(g.toggleFoldoutControl, g.objectName, true);
                if(g.toggleFoldoutControl)
                {                  
                    if (g.groupID != -1)
                    {
                        if (g.actionsToggle[0] =  GUILayout.Toggle(g.actionsToggle[0], g.objectName))
                        {
                            Action[] array = g.GetActions();
                            for (int i = 0; i < array.Length; i++)
                            {
                                Action a = array[i];
                                if (a.Target.Equals(null))
                                {
                                    dirty = true;
                                    return;
                                }
                                ToggleMethod(a);
                                SceneView.RepaintAll();
                            }
                        }
                    }
                    else
                    {
                        //For each method on that object
                        Action[] array = g.GetActions();
                        for (int i = 0; i < array.Length; i++)
                        {
                            Action a = array[i];
                            if (a.Target.Equals(null))
                            {
                                dirty = true;
                                return;
                            }

                            if (g.actionsToggle[i] = GUILayout.Toggle(g.actionsToggle[i], a.Method.Name))
                            {
                                ToggleMethod(a);
                                SceneView.RepaintAll();
                            }
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        void ToggleMethod(Action action)
        {
            if (actions.Contains(action))
            {
                actions.Remove(action);
            }
            else
            {
                actions.Add(action);
            }
        }

        //static public void AddToOnDrawGizmos(string objectName, int id, params Action[] acts)
        //{
        //    GizmoComposite gizmoComposite = gizmoComposites.Find(obj => obj.obj_id == id);
        //    if (gizmoComposite != null)
        //    {
        //        gizmoComposite.AddActions(acts);
        //    }
        //    else
        //    {
        //        gizmoComposites.Add(new GizmoComposite(objectName, id, acts));
        //    }
        //}

        static public void AddToOnDrawGizmos(string objectName, int ObjID, int groupID, Action act)
        {
            if (groupID == -1)
            {
                GizmoComposite gizmoComposite = gizmoComposites.Find(obj => obj.obj_id == ObjID);
                if (gizmoComposite != null)
                {
                    gizmoComposite.AddActions(act);
                }
                else
                {
                    gizmoComposites.Add(new GizmoComposite(objectName, ObjID, groupID, act));
                }
            }
            else
            {
                GizmoComposite gizmoComposite = gizmoComposites.Find(obj => obj.groupID == groupID);
                if (gizmoComposite != null)
                {
                    gizmoComposite.AddActions(act);
                }
                else
                {
                    gizmoComposites.Add(new GizmoComposite(act.Method.Name, ObjID, groupID, act));
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class GizmoMethod : Attribute
    {
        public int groupID;
        public GizmoMethod(int id = -1)
        {
            groupID = id;
        }
    }

    public class GizmoComposite
    {
        Action[] actions;
        public bool[] actionsToggle;
        public string objectName;
        public int obj_id;
        public int groupID;
        public bool toggleFoldoutControl = false;

        public GizmoComposite(string objectName, int id, int gID = -1, params Action[] acts)
        {
            actions = acts;
            this.objectName = objectName;
            obj_id = id;
            this.groupID = gID;
        }

        public Action[] GetActions()
        {
            return actions;
        }

        public void AddActions(params Action[] acts)
        {
            actions = actions.Concat(acts).ToArray();
            actionsToggle = new bool[actions.Length];
        }
    }
}
#elif UNITY_STANDALONE || UNITY_WEBGL
namespace GizmosEditors
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GizmoMethod : Attribute { 
    public int groupID;
        public GizmoMethod(int id = -1)
        {}
    }
}
#endif  
