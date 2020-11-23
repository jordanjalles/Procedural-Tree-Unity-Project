using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralTree : MonoBehaviour
{
    public LSystem lSystem;
    public int iterations = 0;

    [Range(0, 180)]
    public float branchRotateAngle;
    [Range(0, 180)]
    public float branchBendAngle;
    public float branchLengthDefault;

    public float branchDefaultWidth;
    [Range(0f,0.5f)]
    public float taperWidth;
    [Range(0f, 0.5f)]
    public float taperHeight;
    public Vector3 bias;
    public float biasStrength = 0;


    //starting off with some placeholder tree body parts before I procedurally generate them
    public GameObject branchBody;
    public GameObject leafBody;

    public Branch trunk;

    public void ConstructProceduralTree(LSystem lSystem, int iterations, float branchRotateAngle, float branchBendAngle, float branchLengthDefault, float branchDefaultWidth, float taperWidth, float taperHeight, Vector3 bias, float biasStrength, GameObject branchBody, GameObject leafBody)
    {
        this.lSystem = lSystem;
        this.iterations = iterations;
        this.branchRotateAngle = branchRotateAngle;
        this.branchBendAngle = branchBendAngle;
        this.branchLengthDefault = branchLengthDefault;
        this.branchDefaultWidth = branchDefaultWidth;
        this.taperWidth = taperWidth;
        this.taperHeight = taperHeight;
        this.branchBody = branchBody;
        this.leafBody = leafBody;
        this.bias = bias;
        this.biasStrength = biasStrength;


        IterateDNA();
        DrawTree();
        
    }

    void OnDrawGizmosSelected()
    {
        
        foreach (Branch branch in getBranchesRecursively(trunk))
        {
            Vector3 axis = Vector3.Cross(branch.body.transform.up, bias);
            float angle = Vector3.Angle(branch.body.transform.up, bias) * 0.5f;
            
            Gizmos.color = Color.green;
            Gizmos.DrawRay(branch.body.transform.position, axis);
            
            Gizmos.color = Color.red;
            Gizmos.DrawRay(branch.body.transform.position, branch.body.transform.forward);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(branch.body.transform.position, branch.bias);

        }
        
    }

    List<Branch> getBranchesRecursively(Branch branch)
    {
        List<Branch> branches = new List<Branch>();
        branches.Add(branch);

        if (branch.childBranches.Count > 0)
        {
            foreach (Branch childBranch in branch.childBranches)
            {
                branches.AddRange((getBranchesRecursively(childBranch)));
            }
        }
        return branches;
    }

    void IterateDNA()
    {
        for (int i = 0; i < iterations; i++)
        {
            lSystem.currentString = lSystem.Iterate();
        }
        //Debug.Log(lSystem.currentString);

    }

    // Update is called once per frame
    void DrawTree()
    { 
        //branch head is like the turtle cursor, only need to save one at a time to immitate current turtle position
        Branch branchHead = new Branch(transform.position, 0, 0, branchBody, transform, 0);
        trunk = branchHead;
        branchHead.bodyMesh.SetActive(false);//deactivate the root branch body b/c this is just for getting started
        

        
        //branch queue lets us save branches for returning later
        Stack<Branch> branchStack = new Stack<Branch>();

        for (int i = 0; i < lSystem.currentString.Length; i++)
        {
            char instruction = lSystem.currentString[i];
            if (instruction == '0')
            {
                //create a branch at previous branch tip, (and a leaf, someday)

                //new branch 
                branchHead = GrowBranch(branchHead, branchLengthDefault, branchDefaultWidth); 
                GrowLeaf(branchHead, branchDefaultWidth, branchDefaultWidth);

            }
            else if(instruction == '1')
            {
                //create a branch at previous branch tip

                //new branch
                branchHead = GrowBranch(branchHead, branchLengthDefault, branchDefaultWidth);

            }
            else if (instruction == '[')
            {

                //(enqueue) save this branch because we will return to it later
                branchStack.Push(branchHead);
            }
            else if (instruction == ']')
            {
                //(dequeue) return to last saved branch and forget it
                branchHead = branchStack.Pop();
            }
            else if (instruction == '+')
            {
                //bend forward
                branchHead.Bend(branchBendAngle);
            }
            else if (instruction == '-')
            {
                //bend backwards
                branchHead.Bend(-branchBendAngle);
            }else if (instruction == 'R')
            {
                //rotate about Y axis
                branchHead.RotateY(branchRotateAngle);
            }
            else if (instruction == 'L')
            {
                //rotate about Y axis
                branchHead.RotateY(-branchRotateAngle);
            }
            else if (instruction == 'B')
            {
                branchHead.BiasTowards(bias, biasStrength);
            }
        }
        UpdateBranchRecursively(trunk);
        
    }

    private Branch GrowBranch(Branch parentBranch, float length, float width)
    {
        float branchWidth = width * Mathf.Pow((1 - taperWidth), parentBranch.depth);
        float branchLength = length * Mathf.Pow((1 - taperHeight), parentBranch.depth);
        Branch branch = new Branch(parentBranch.tipPos, branchLength, branchWidth, branchBody, parentBranch.body.transform, parentBranch.depth);
        parentBranch.childBranches.Add(branch);
        return branch;
    }

    private Branch GrowLeaf(Branch parentBranch, float length, float width)
    {
        float branchWidth = width * Mathf.Pow((1 - taperWidth), parentBranch.depth);
        float branchLength = length * Mathf.Pow((1 - taperHeight), parentBranch.depth);
        return new Branch(parentBranch.tipPos, branchLength, branchWidth, leafBody, parentBranch.body.transform, parentBranch.depth);
    }

    public void UpdateBranchRecursively(Branch branch)
    {

            branch.ApplyBias();
        
        if (branch.childBranches.Count > 0)
        {
            foreach(Branch childBranch in branch.childBranches)
            {
                
                UpdateBranchRecursively(childBranch);
            }
        }
    }

    

    public class Branch
    {
        public float length;
        public float width;
        public GameObject bodyMesh; //object that gets created and scaled to show branch
        public GameObject body; //parent of body mesh to create rotation node for branch children
        public int depth;
        public bool hasBias = false;
        
        public List<Branch> childBranches = new List<Branch>();
        public Vector3 bias = Vector3.up;
        public float biasStrength = 0;

        public Branch(Vector3 pos, float length, float width, GameObject bodyMesh, Transform parent, int parentDepth)
        {
            this.length = length;
            this.width = width;
            this.depth = parentDepth + 1;
            this.body = new GameObject("Branch" + parentDepth.ToString());
            body.transform.position = pos;
            body.transform.parent = parent;
            body.transform.rotation = parent.rotation;

            this.bodyMesh = Instantiate(bodyMesh, body.transform) as GameObject;
            this.bodyMesh.transform.localScale = new Vector3(width, length/2, width);
            //using length/2 b/c cylinder primitive is 2 units tall
        }

        

        public Vector3 tipPos
        {
            get
            {
                return body.transform.position + body.transform.up * length;
            }
        }

        //rotates about Y axis
        public void RotateY(float angle)
        {
            body.transform.Rotate(body.transform.up, angle);
        }

        //rotates about X axis
        public void Bend(float angle)
        {
            body.transform.Rotate(body.transform.right, angle);
        }

        public void BiasTowards(Vector3 bias, float biasStrength)
        {
            hasBias = true;
            this.bias = bias.normalized;
            Mathf.Clamp(biasStrength, 0, 1);
            this.biasStrength = biasStrength;
        }

        public void ApplyBias()
        {
            if (hasBias) {
                Vector3 axis = Vector3.Cross(body.transform.up, bias);
                axis = body.transform.InverseTransformDirection(axis);

                float angle = Vector3.Angle(body.transform.up, bias)*biasStrength;
                body.transform.Rotate(axis, angle);
            }
        }
    }
}
