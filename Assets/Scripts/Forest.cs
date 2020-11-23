using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forest : MonoBehaviour
{
    public int rows;
    public int columns;
    public int distanceBetweenTrees;
    public int seed;
    public int iterations;

    public GameObject branchBody;
    public GameObject leafBody;

    private System.Random prng;


    // Start is called before the first frame update
    void Start()
    {
        prng = new System.Random(seed);
        RandomTreeGrid();
        //tree = RandomTree(new Vector3(0, 0, 0), iterations);
    }

        

    void RandomTreeGrid()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                RandomTree(new Vector3(x*distanceBetweenTrees, 0, z*distanceBetweenTrees), iterations);
            }
        }
    }

    ProceduralTree RandomTree(Vector3 pos, int iterations) {

        //binary tree
        //LSystem lSystem = new LSystem("0", new Dictionary<string, string> {  {"0", "1B[[0R-]0R+]" } });
        
        //Lopsided binary tree
        //LSystem lSystem = new LSystem("0", new Dictionary<string, string> {  { "0", "1B[[10R-]0R+]" } });

        //simple vine
        LSystem lSystem = new LSystem("1R10", new Dictionary<string, string> { { "1", "1BR[0-]1" } });

        //triple vine
        //LSystem lSystem = new LSystem("10", new Dictionary<string, string> { { "1", "1BR[0R-][0+][0L-]1" } });

        //Trinary tree
        //LSystem lSystem = new LSystem("0", new Dictionary<string, string> {  {"0", "1[[0R-][1B0+][0R+]]" } });

        //Trinary tree lopsided
        //LSystem lSystem = new LSystem("0", new Dictionary<string, string> { {"0", "1B[110R-][10R][0R+]" } });
        //LSystem lSystem = new LSystem("0", new Dictionary<string, string> { {"0", "1RB[110-][10][0+]" } });



        //alternating binary tree
        //LSystem lSystem = new LSystem("0", new Dictionary<string, string> { { "+", "-" }, { "-", "+" }, { "0", "1B[[0R+]0L+]" } });

        //alternating trinary tree <3
        //LSystem lSystem = new LSystem("0", new Dictionary<string, string> { { "R", "L" }, { "L", "R" }, { "0", "1[[0L+][10-][0R+]]" } });


        float startRotation = prng.Next(0, 360);
 
        float branchRotateAngle = SampleBellCurveInRange(0f, 180f);

        float branchBendAngle = SampleBellCurveInRange(0f, 90f);

        float branchLengthDefault = SampleBellCurveInRange(1f, 4f);

        float branchDefaultWidth = SampleBellCurveInRange(0.5f, 2f);

        float taperWidth = SampleBellCurveInRange(0.1f, 0.5f);

        float taperHeight = SampleBellCurveInRange(0f, 0.5f);

        Vector3 bias = RandomNormalizedVector();
        float biasStrength = SampleBellCurveInRange(0f, 0.2f);

        GameObject treeBase = new GameObject("TreeBase");
        treeBase.transform.Rotate(treeBase.transform.up, startRotation);

        treeBase.transform.position = pos;
        treeBase.transform.parent = transform;
        ProceduralTree tree = treeBase.AddComponent<ProceduralTree>();
        tree.ConstructProceduralTree(lSystem, iterations, branchRotateAngle, branchBendAngle, branchLengthDefault, branchDefaultWidth, taperWidth, taperHeight, bias, biasStrength, branchBody, leafBody);
        return tree;
    }

    float RandomFloatInRange(float min, float max)
    {
        return ((float)prng.NextDouble() + min) * (max - min);
    }

    float SampleBellCurveInRange(float min, float max)
    {
        return (RandomFloatInRange(min, max) + RandomFloatInRange(min, max)) / 2;
    }

    Vector3 RandomNormalizedVector()
    {
        return new Vector3(prng.Next(-100, 100), prng.Next(-100, 100), prng.Next(-100, 100)).normalized;
    }
}
