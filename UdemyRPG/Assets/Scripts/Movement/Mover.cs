using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using System.Collections.Generic;
using RPG.Resources;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 6f;
        NavMeshAgent navMeshAgent;
        ActionScheduler actionScheduler;
        Health health;

        [System.Serializable]
        struct MoverSaveData{
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        private void Awake() {
            navMeshAgent = GetComponent<NavMeshAgent>();
            actionScheduler = GetComponent<ActionScheduler>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            updateAnimator();
        }

        public void Cancel(){
            navMeshAgent.isStopped = true;
        }

        public void MoveBeforeAttack(Vector3 destination, float percentageSpeed){
            actionScheduler.StartAction(this);
            MoveCharTo(destination, percentageSpeed);
        }

        public void MoveTo(Vector3 destination, float percentageSpeed){
            actionScheduler.StartAction(this);
            MoveCharTo(destination, percentageSpeed);
        }

        public void MoveCharTo(Vector3 destination, float percentageSpeed)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(percentageSpeed);
        }

        private void updateAnimator()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(navMeshAgent.velocity);
            GetComponent<Animator>().SetFloat("forwardSpeed", localVelocity.z);
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData) state;
            navMeshAgent.Warp(data.position.ToVector());
            transform.eulerAngles = data.rotation.ToVector();
        }
    }

}