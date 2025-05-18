using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace SSS.System.Echo
{
    
    public class EchoGenerator : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMeshRenderer; // 복사 대상
        public Material echoMaterial; // 잔상 표현용 머티리얼
        
        public GameObject echoPrefab;
        
        [Header("Echo Color")]
        public Animator animator;
        [SerializeField] private Color echoColor;
        private static readonly int IsColorLoopOn = Animator.StringToHash("IsColorLoop");
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        void Start()
        {
            if (animator)
            {
                animator.SetBool(IsColorLoopOn, true);
                // animator.Play("CharacterTrailColorLoop");
            }
        }
        
        public void CreateEcho(float echoDuration)
        {
            if (!skinnedMeshRenderer) return;
            if (!echoPrefab) return;

            // 현재 포즈를 복사
            Mesh bakedMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(bakedMesh);

            // 오브젝트 생성
            var echo = Instantiate(echoPrefab);
            echo.transform.position = skinnedMeshRenderer.transform.position;
            echo.transform.rotation = skinnedMeshRenderer.transform.rotation;
            echo.transform.localScale = skinnedMeshRenderer.transform.lossyScale;

            // 메시와 머티리얼 세팅
            var mf = echo.GetComponent<MeshFilter>();
            var mr = echo.GetComponent<MeshRenderer>();

            mf.sharedMesh = bakedMesh;

            int subMeshCount = bakedMesh.subMeshCount;
            // 모든 SubMesh에 동일한 머티리얼 적용
            var materials = Enumerable.Repeat(echoMaterial, subMeshCount).ToArray();
            mr.sharedMaterials = materials;
            
            // 머티리얼 프로퍼티 블럭 설정
            var mpb = new MaterialPropertyBlock();
            mpb.SetColor(BaseColor, echoColor);
            for (int i = 0; i < subMeshCount; ++i)
                mr.SetPropertyBlock(mpb, i); // 서브메시 단위로 설정

            Destroy(echo, echoDuration);
        }
    }
}