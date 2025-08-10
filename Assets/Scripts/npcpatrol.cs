using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class npcpatrol : MonoBehaviour
    {
        public Transform[] patrolPoints; // Array titik patrol
        public float speed = 2f; // Kecepatan NPC
        private Transform targetPoint; // Titik patrol tujuan
        private bool isWaiting = false; // Apakah NPC sedang menunggu
        public float waitTime = 2f; // Waktu menunggu di setiap titik
        private Animator animControl;
        private bool isMoving = false; // Menambahkan variabel untuk mengecek apakah NPC sedang bergerak

        void Start()
        {
            if (patrolPoints.Length > 0)
            {
                SetRandomPatrolPoint();
            }
            else
            {
                Debug.LogError("Tidak ada titik patrol yang ditetapkan.");
            }
            animControl = GetComponent<Animator>();
        }

        void Update()
        {
            if (targetPoint != null && !isWaiting)
            {
                Patrol();
                isMoving = true; // Set NPC sedang bergerak
            }
            else
            {
                isMoving = false; // Set NPC sedang diam
            }
        }

        void Patrol()
        {
            // Menghitung arah pergerakan NPC
            Vector3 direction = (targetPoint.position - transform.position).normalized;

            // Mengatur rotasi NPC agar menghadap ke arah pergerakan
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speed);
            }

            // Pindah ke titik patrol saat ini
            transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

            // Jika NPC sudah mencapai titik patrol, berhenti sejenak sebelum memilih titik patrol acak berikutnya
            if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
            {
                isMoving = false; // NPC berhenti bergerak saat mencapai titik patrol
                StartCoroutine(WaitBeforeNextPatrol());
            }
        }

        IEnumerator WaitBeforeNextPatrol()
        {
            isWaiting = true;
            yield return new WaitForSeconds(waitTime);
            SetRandomPatrolPoint();
            isWaiting = false;
        }

        void SetRandomPatrolPoint()
        {
            int randomIndex = Random.Range(0, patrolPoints.Length);
            targetPoint = patrolPoints[randomIndex];
        }

        // Fungsi untuk menggambar garis di editor Unity
        void OnDrawGizmos()
        {
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (patrolPoints[i] != null)
                    {
                        Gizmos.DrawWireSphere(patrolPoints[i].position, 0.3f);

                        if (i + 1 < patrolPoints.Length && patrolPoints[i + 1] != null)
                        {
                            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                        }
                    }
                }
            }
        }
    }
}
