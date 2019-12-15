using UnityEngine;

public class Simulator : MonoBehaviour
{
    public int num_iter;
    public RenderTexture renderTexture;
    public Vector3Int simulationSpaceSize;

    public ComputeShader shader;
    public ComputeBuffer r,r1,u,u1;
    enum KernelId
    {
        R=0,
        U=1,
        Update=2,
        Output=3,
    }
    void SetupComputeShader()
    {
        if (r != null) r.Release();
        if (r1 != null) r1.Release();
        if (u != null) u.Release();
        if (u1 != null) u1.Release();
        int count = simulationSpaceSize.x * simulationSpaceSize.y * simulationSpaceSize.z;
        r = new ComputeBuffer(count, sizeof(float));
        u = new ComputeBuffer(count, sizeof(float)*3);
        
    }
    void Update()
    {
        for(int i = 0; i < num_iter; i++)
        {
            
        }
    }
}
