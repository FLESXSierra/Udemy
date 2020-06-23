using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Resources;

namespace RPG.Controller.AI
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspeciousTime = 4;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDelayTime = 3f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        CombatController combatController;
        Health health;
        Vector3 guardPoint;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArraiveAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex=0;

        private void Awake() {
            combatController = GetComponent<CombatController>();
            health = GetComponent<Health>();
        }

        private void Start() {
            guardPoint = transform.position;
        }

        private void Update() {
            if(!health.IsDead()){
                GameObject player=GameObject.FindWithTag("Player");
                float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
                if(chaseDistance > distanceToPlayer && !combatController.IsTargetDead(player)){
                    timeSinceLastSawPlayer = 0;
                    combatController.Attack(player);
                }
                else if (timeSinceLastSawPlayer<suspeciousTime){
                    GetComponent<ActionScheduler>().DisableCurrentAction();
                }
                else{
                    PatrolBehavior();
                }
               UpdateTimes();
            }
        }

        private void UpdateTimes(){
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArraiveAtWaypoint += Time.deltaTime;
        }
        
        private void PatrolBehavior(){
            Vector3 nextPosition = guardPoint;
            if(patrolPath != null){
                if(AtWayPoint()){
                    timeSinceArraiveAtWaypoint = 0;
                    CycleWayPoint();
                }
                nextPosition = GetCurrentWayPoint();
            }
            if(timeSinceArraiveAtWaypoint> waypointDelayTime){
                GetComponent<Mover>().MoveTo(nextPosition, patrolSpeedFraction);
            }
        }

        private bool AtWayPoint(){
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWaypoint<waypointTolerance;
        }

        private void CycleWayPoint(){
            currentWaypointIndex = patrolPath.GetNextPatrolPoint(currentWaypointIndex);
        }

        private Vector3 GetCurrentWayPoint(){
            return patrolPath.GetPatrolPointByIndex(currentWaypointIndex);
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,chaseDistance);
        }
    }   
}
