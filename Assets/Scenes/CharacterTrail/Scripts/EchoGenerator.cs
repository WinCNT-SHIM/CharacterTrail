using System.Linq;
using UnityEngine;

namespace SSS.System.Echo
{
    
    public class EchoGenerator : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMeshRenderer; // 복사 대상
        public Material ghostMaterial; // 잔상 표현용 머티리얼
        
        public GameObject echoPrefab;
        
        public void CreateEcho(float echoDuration)
        {
            if (!skinnedMeshRenderer) return;
            if (!echoPrefab) return;

            // 현재 포즈를 복사
            Mesh bakedMesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(bakedMesh);

            // // 오브젝트 생성
            var echo = Instantiate(echoPrefab);
            echo.transform.position = skinnedMeshRenderer.transform.position;
            echo.transform.rotation = skinnedMeshRenderer.transform.rotation;
            echo.transform.localScale = skinnedMeshRenderer.transform.lossyScale;

            // 메시와 머티리얼 세팅
            var mf = echo.GetComponent<MeshFilter>();
            var mr = echo.GetComponent<MeshRenderer>();

            mf.sharedMesh = bakedMesh;

            int subMeshCount = bakedMesh.subMeshCount;
            // // 모든 SubMesh에 동일한 머티리얼 적용
            var materials = Enumerable.Repeat(ghostMaterial, subMeshCount).ToArray();
            mr.sharedMaterials = materials;

            Destroy(echo, echoDuration);
        }
    }
}