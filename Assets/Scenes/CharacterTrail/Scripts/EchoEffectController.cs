using System.Collections;
using UnityEngine;

namespace SSS.System.Echo
{
    public class EchoEffectController : MonoBehaviour
    {
        [Header("Echo Settings")]
        public EchoGenerator echoGenerator;
        [Tooltip("에코 생성 간격")]
        [SerializeField] private float interval = 0.05f;
        [Tooltip("각 에코의 생존 시간")]
        [SerializeField] private float echoDuration = 1.0f;

        private Coroutine echoRoutine;

        public void StartEcho()
        {
            if (echoRoutine == null)
                echoRoutine = StartCoroutine(EchoLoop());
        }

        public void StopEchoLoop()
        {
            if (echoRoutine != null)
            {
                StopCoroutine(echoRoutine);
                echoRoutine = null;
            }
        }

        private IEnumerator EchoLoop()
        {
            while (true)
            {
                echoGenerator.CreateEcho(echoDuration);
                yield return new WaitForSeconds(interval);
            }
        }
    }
}