using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using UnityEngine.AI;
using RPG.Resources;
using System;
using UnityEngine.EventSystems;

namespace RPG.Controller
{
    public class PlayerController : MonoBehaviour
    {

        [System.Serializable]
        struct CursorMapping{
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float speedPercentage = 1f;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float maxNavMeshPathLength = 40f;
        Mover mover;
        NavMeshAgent navMeshAgent;
        CombatController combatController;
        Health health;

        void Awake(){
            mover = GetComponent<Mover>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            combatController = GetComponent<CombatController>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            if(InteractWithUI()) return;
            if(health.IsDead()){
                SetCursor(CursorType.None);
                return;
            } 
            if(InteractWithComponents()) return;
            if(InteractWithMovement()) return;
            SetCursor(CursorType.Invalid);
        }

        private bool InteractWithComponents()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                if(raycastables != null){
                    foreach (IRaycastable raycastable in raycastables)
                    {
                        if(raycastable.HandleRaycast(this)){
                            SetCursor(raycastable.GetCursorType(this));
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool InteractWithUI()
        {
            if(EventSystem.current.IsPointerOverGameObject()){
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private RaycastHit[] RaycastAllSorted(){
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithMovement()
        {
            bool isMoving = false;
            if (Camera.main != null)
            {
                Vector3 target;
                bool hasHit = RaycastNavMesh(out target);
                if (hasHit)
                {
                    isMoving = true;
                    SetCursor(CursorType.None);
                    if(IsMouseDown()){
                        mover.MoveTo(target, speedPercentage);
                        SetCursor(CursorType.Movement);
                    }
                }
            }
            return isMoving;
        }

        private bool RaycastNavMesh(out Vector3 target) {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if(hasHit){
                NavMeshHit navMeshHit;
                bool castToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
                if(castToNavMesh){
                    target = navMeshHit.position;
                    NavMeshPath navMeshPath = new NavMeshPath();
                    bool havePath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, navMeshPath);
                    if(havePath && navMeshPath.status == NavMeshPathStatus.PathComplete && 
                       GetPathLength(navMeshPath) < maxNavMeshPathLength){
                        return true;
                    }
                }
            }
            return false;
        }

        private float GetPathLength(NavMeshPath navMeshPath)
        {
            float total=0;
            Vector3[] corners = navMeshPath.corners;
            if(corners.Length>=2){
                for (int i = 0; i < corners.Length - 1; i++)
                {
                    total += Vector3.Distance(corners[i],corners[i+1]); 
                }
            }
            return total;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type){
            foreach (CursorMapping mapping in cursorMappings)
            {
                if(type == mapping.type){
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private static bool IsMouseDown()
        {
            return Input.GetMouseButton(0);
        }

        private static bool IsShiftKeyDown()
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
    }
}
