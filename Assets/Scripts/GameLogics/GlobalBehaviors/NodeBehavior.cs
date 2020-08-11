using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class NodeBehavior : MonoBehaviour
{
    GameObject[] nodes;
    public float particleSizeMax;
    public float partGrowSpeed;
    public float partShrinkSpeed;
    private float particleSizeInitial;
    private bool[] particleSizing;
    private bool[] particleShrinking;

    public float collGrowSpeed;
    public float collShrinkSpeed;
    public float collMaxScale;
    public float collMinScale;
    float nodeNormalSize;
    bool[] nodeColl;
    bool[] pleaseNormalizeNode;
    float[] collSizingSpeed;
    bool[] spawning;

    public Color colorGray, colorRed, colorYellow, colorGreen;
    public Color particleRed, particleYellow, particleGreen, particleNeutral;

    Animator[] nodeDriftMovement;

    public enum NodeState
    {
        Spawning,
        NotTarget,
        Target,
        TargetCompleted
    }
    public NodeState[] nodeStates;

    public void SpawnNodes(bool firstTime)
    {
        if (firstTime)
        {
            nodes = LevelManager.nodes;
            nodeNormalSize = nodes[0].transform.localScale.x;
            spawning = new bool[nodes.Length];
            nodeColl = new bool[nodes.Length];
            pleaseNormalizeNode = new bool[nodes.Length];
            collSizingSpeed = new float[nodes.Length];
            particleSizing = new bool[nodes.Length];
            particleShrinking = new bool[nodes.Length];
            nodeDriftMovement = new Animator[nodes.Length];
            nodeStates = new NodeState[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                //nodeParticleStatic[i] = Instantiate(nodeParticleStaticPrefab, nodes[i].transform);
            }
        }
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].transform.localScale = new Vector3(0, 0, 0);
            spawning[i] = true;

            NodeParticles np = nodes[i].GetComponentInChildren<NodeParticles>();
            np.psStatic.gameObject.SetActive(true);

            //nodeParticleStatic[i].SetActive(true);
            nodes[i].GetComponentInChildren<NodeCometHaloAnimation>().Spawn();
            nodes[i].GetComponentInChildren<MeshRenderer>().enabled = true;
        }
        RemoveTargetHighlighting(LevelManager.targetNodes[0]);
    }
    public void AllAppear(bool appear)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            NodeParticles np = nodes[i].GetComponentInChildren<NodeParticles>();
            np.psStatic.gameObject.SetActive(appear);
            //nodeParticleStatic[i].SetActive(appear);
            
            nodeDriftMovement[i] = nodes[i].GetComponentInChildren<Animator>();
            nodeDriftMovement[i].enabled = false;
            if (appear)
            {
                StartCoroutine(PlayNodeDriftAnimation(i));
            }
            else
            {
                nodes[i].GetComponentInChildren<NodeCometHaloAnimation>().Despawn();
                nodes[i].GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
    }
    IEnumerator PlayNodeDriftAnimation(int i)
    {
        yield return new WaitForSeconds(i * 0.2f);
        nodeDriftMovement[i].enabled = true;
        //nodes[i].GetComponentInParent<Animator>().Play(0);
    }
    
    public void AllExplode()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            NodeParticles np = nodes[i].GetComponentInChildren<NodeParticles>();
            np.psSpawnA.Play();
            np.psSpawnB.Play();
            //nodeParticleSpawnA[i].GetComponent<ParticleSystem>().Play();
            //nodeParticleSpawnB[i].GetComponent<ParticleSystem>().Play();
        }
    }
    public int activationAnimLookahead;
    public void ObjectiveActivationAnimation(int nodeIndex)
    {
        NodeParticles np = nodes[nodeIndex].GetComponentInChildren<NodeParticles>();
        np.psInwardsCircle.Play();
    }
    public void TargetExplode(int nodeIndex)
    {
        NodeParticles np = nodes[nodeIndex].GetComponentInChildren<NodeParticles>();
        np.psSpawnA.Play();
        np.psSpawnB.Play();
        //nodeParticleSpawnA[nodeIndex].GetComponent<ParticleSystem>().Play();
        //nodeParticleSpawnB[nodeIndex].GetComponent<ParticleSystem>().Play();
    }
    public void LoadLevel()
    {
        if (nodes != null) // is this necessary?
        {
            if (GameManager.loadNewLevel)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    if (nodes[i].GetComponent<ParticleSystem>() == null)
                    {
                        NodeParticles np = nodes[i].GetComponentInChildren<NodeParticles>();
                        np.psCollComet.Stop();
                        np.psCollOrb.Stop();
                        np.psSpawnA.Play();
                        np.psSpawnB.Play();

                        //nodeParticleCometColl[i] = Instantiate(nodeParticleCometCollPrefab, nodes[i].transform);
                        //nodeParticleCometColl[i].SetActive(true);
                        //nodeParticleCometColl[i].GetComponent<ParticleSystem>().Stop();

                        //nodeParticleEnergySphereColl[i] = Instantiate(nodeParticleEnergySphereCollPrefab, nodes[i].transform);
                        //nodeParticleEnergySphereColl[i].SetActive(true);
                        //nodeParticleEnergySphereColl[i].GetComponent<ParticleSystem>().Stop();

                        //nodeParticleSpawnA[i] = Instantiate(nodeParticleSpawnAPrefab, nodes[i].transform);
                        //nodeParticleSpawnA[i].SetActive(true);

                        //nodeParticleSpawnB[i] = Instantiate(nodeParticleSpawnBPrefab, nodes[i].transform);
                        //nodeParticleSpawnB[i].SetActive(true);
                    }
                }
            }
            particleSizeInitial = nodes[0].GetComponentInChildren<NodeParticles>().psStatic.main.startSize.constant;

            //ParticleSystem.MainModule psmain = nodeParticleStaticPrefab.GetComponent<ParticleSystem>().main;
            //particleSizeInitial = psmain.startSize.constant;

            AllExplode();
            AllAppear(true);
        }
    }

    public float spawnGrowthRate;
    private void FixedUpdate()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (spawning[i])
                {
                    nodes[i].transform.localScale += new Vector3(1,1,1) * spawnGrowthRate;
                    if (nodes[i].transform.localScale.x > nodeNormalSize)
                    {
                        nodes[i].transform.localScale = new Vector3(1, 1, 1) * nodeNormalSize;
                        spawning[i] = false;
                    }
                }
                else
                {
                    if (nodeColl[i])
                    {
                        float scaleX = nodes[i].transform.localScale.x;
                        if (scaleX > nodeNormalSize + collMaxScale)
                        {
                            collSizingSpeed[i] = -collShrinkSpeed;
                        }
                        else if (scaleX < nodeNormalSize - collMinScale)
                        {
                            collSizingSpeed[i] = collShrinkSpeed;
                            pleaseNormalizeNode[i] = true;
                        }
                        if (pleaseNormalizeNode[i] && scaleX >= nodeNormalSize)
                        {
                            pleaseNormalizeNode[i] = false;
                            collSizingSpeed[i] = 0;
                            nodes[i].transform.localScale = new Vector3(1, 1, 1) * nodeNormalSize;
                            nodeColl[i] = false;
                        }
                        nodes[i].transform.localScale += new Vector3(1, 1, 1) * collSizingSpeed[i];
                    }
                    if (particleSizing[i])
                    {
                        NodeParticles np = nodes[i].GetComponentInChildren<NodeParticles>();
                        ParticleSystem.MainModule psmStatic = np.psStatic.main;

                        //ParticleSystem.MainModule psmain = nodeParticleStatic[i].GetComponent<ParticleSystem>().main;
                        float size = psmStatic.startSize.constant;
                        if (particleShrinking[i])
                        {
                            size -= partShrinkSpeed;
                            if (size < particleSizeInitial)
                            {
                                size = particleSizeInitial;
                                particleShrinking[i] = false;
                                particleSizing[i] = false;
                            }
                        }
                        else
                        {
                            size += partGrowSpeed;
                            if (size > particleSizeMax)
                            {
                                size = particleSizeMax;
                                particleShrinking[i] = true;
                            }
                        }
                        psmStatic.startSize = size;
                    }
                }
            }
        }
    }

    public void CollisionNodeCometColor(int nodeIndex, bool target, bool first, bool objCompleted)
    {
        NodeParticles np = nodes[nodeIndex].GetComponentInChildren<NodeParticles>();
        ParticleSystem psComet = np.psCollComet;
        ParticleSystem.MainModule psmComet = psComet.main;

        //ParticleSystem psComet = nodeParticleCometColl[nodeIndex].GetComponent<ParticleSystem>();
        //ParticleSystem.MainModule psmComet = psComet.main;
        
        if (target)
        {
            if (first)
            {
                psmComet.startColor = particleYellow;
            }
            else if (objCompleted)
            {
                psmComet.startColor = particleGreen;
            }
            else
            {
                psmComet.startColor = particleRed;
            }
        }
        else
        {
            psmComet.startColor = particleNeutral;
        }

        nodeColl[nodeIndex] = true;
        pleaseNormalizeNode[nodeIndex] = false;
        collSizingSpeed[nodeIndex] = collGrowSpeed;
        psComet.Play();
        //nodeParticleEnergySphereColl[nodeIndex].GetComponent<ParticleSystem>().Play();
        particleSizing[nodeIndex] = true;

        nodes[nodeIndex].GetComponentInChildren<NodeCometHaloAnimation>().CollComet(target, first, objCompleted);
    }

    public void CollisionNodeEnergySphereColor(GameObject node, bool correct)
    {
        
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] == node)
            {
                NodeParticles np = nodes[i].GetComponentInChildren<NodeParticles>();
                ParticleSystem ps = np.psCollOrb;
                ParticleSystem.MainModule psm = ps.main;
                //ParticleSystem ps = nodeParticleEnergySphereColl[i].GetComponent<ParticleSystem>();
                //ParticleSystem.MainModule psm = ps.main;

                if (correct)
                    psm.startColor = particleGreen;
                else
                    psm.startColor = particleRed;
                
                nodeColl[i] = true;
                pleaseNormalizeNode[i] = false;
                collSizingSpeed[i] = collGrowSpeed;
                //nodeParticleCometColl[i].GetComponent<ParticleSystem>().Play();
                ps.Play();
                particleSizing[i] = true;

                nodes[i].GetComponentInChildren<NodeCometHaloAnimation>().CollOrb(correct, false);
                nodes[i].GetComponentInChildren<NodeOrbHaloAnimation>().CollOrb(correct, false);
            }
        }
        
    }
    //public void CollisionNodeEnergySphere(GameObject node)
    //{
    //    for (int i = 0; i < nodes.Length; i++)
    //    {
    //        if (nodes[i] == node)
    //        {
    //            nodeColl[i] = true;
    //            pleaseNormalizeNode[i] = false;
    //            collSizingSpeed[i] = collGrowSpeed;
    //            //nodeParticleCometColl[i].GetComponent<ParticleSystem>().Play();
    //            nodeParticleEnergySphereColl[i].GetComponent<ParticleSystem>().Play();
    //            particleSizing[i] = true;
    //        }
    //    }
    //}

    

    public void RemoveTargetHighlighting(int targetIndex)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodeStates[i] = NodeState.NotTarget;
            Material nodeMat = LevelManager.nodes[i].GetComponentInChildren<MeshRenderer>().material;
            nodeMat.color = colorGray;

            NodeParticles np = nodes[i].GetComponentInChildren<NodeParticles>();
            ParticleSystem.MainModule psmain = np.psStatic.main;

            //ParticleSystem.MainModule psmain = nodeParticleStatic[i].GetComponent<ParticleSystem>().main;
            psmain.startColor = colorRed;
            nodes[i].GetComponentInChildren<NodeCometHaloAnimation>().HighlightNewTarget(false);
        }
    }
    public void HighlightNewTarget(int targetIndex)
    {
        nodeStates[targetIndex] = NodeState.Target;
        Material nodeMat = LevelManager.nodes[targetIndex].GetComponentInChildren<MeshRenderer>().material;
        nodeMat.color = colorYellow;

        NodeParticles np = nodes[targetIndex].GetComponentInChildren<NodeParticles>();
        ParticleSystem.MainModule psmStatic = np.psStatic.main;

        //ParticleSystem.MainModule psmain = nodeParticleStatic[targetIndex].GetComponent<ParticleSystem>().main;
        psmStatic.startColor = colorYellow;

        ParticleSystem psCollOrb = np.psCollOrb;
        ParticleSystem.MainModule psmCollOrb = psCollOrb.main;

        //ParticleSystem psCollOrb = nodeParticleEnergySphereColl[targetIndex].GetComponent<ParticleSystem>();
        //ParticleSystem.MainModule psmCollOrb = psCollOrb.main;
        psmCollOrb.startColor = particleYellow;
        psCollOrb.Play();
        //nodeParticleEnergySphereColl[targetIndex].GetComponent<ParticleSystem>().Play();

        nodes[targetIndex].GetComponentInChildren<NodeCometHaloAnimation>().HighlightNewTarget(true);
    }

    public void HighlightCompletedTarget(int targetIndex)
    {
        nodeStates[targetIndex] = NodeState.TargetCompleted;
        Material nodeMat = LevelManager.nodes[targetIndex].GetComponentInChildren<MeshRenderer>().material;
        nodeMat.color = colorGreen;

        NodeParticles np = nodes[targetIndex].GetComponentInChildren<NodeParticles>();
        ParticleSystem.MainModule psmStatic = np.psStatic.main;

        //ParticleSystem.MainModule psmain = nodeParticleStatic[targetIndex].GetComponent<ParticleSystem>().main;
        psmStatic.startColor = colorGreen;

        LevelManager.nodes[targetIndex].GetComponentInChildren<NodeCometHaloAnimation>().CollOrb(true, true);
        LevelManager.nodes[targetIndex].GetComponentInChildren<NodeOrbHaloAnimation>().CollOrb(true, true);
    }

    public void ApplyColorsParticleEvents(Color cBadColl, Color cGoodColl, Color cCometCollNodeNotTarget)
    {
        particleRed = cBadColl;
        particleGreen = cGoodColl;
        particleNeutral = cCometCollNodeNotTarget;

        //spawning and exploding: nodeParticleSpawnAPrefab
        //spawning and exploding: nodeParticleSpawnBPrefab
    }

    public void ApplyColors(Color cNotTarget, Color cTarget, Color cCompleted, Color cNotTargetParticle)
    {
        colorGray = cNotTarget;
        colorYellow = cTarget;
        particleYellow = cTarget;
        colorGreen = cCompleted;
        colorRed = cNotTargetParticle;
        
        if (nodeStates.Length == 0)
        {
            print("null");
        }
        else
        {
            print("not null");
            for (int i = 0; i < nodes.Length; i++)
            {
                Material nodeMat = LevelManager.nodes[i].GetComponentInChildren<MeshRenderer>().material;
                //ParticleSystem.MainModule psmain = nodeParticleStatic[i].GetComponent<ParticleSystem>().main;

                NodeParticles np = nodes[i].GetComponentInChildren<NodeParticles>();
                ParticleSystem.MainModule psmStatic = np.psStatic.main;

                if (nodeStates[i] == NodeState.NotTarget)
                {
                    nodeMat.color = colorGray;
                    psmStatic.startColor = colorRed;
                }
                if (nodeStates[i] == NodeState.Target)
                {
                    nodeMat.color = colorYellow;
                    psmStatic.startColor = colorYellow;
                }
                if (nodeStates[i] == NodeState.TargetCompleted)
                {
                    nodeMat.color = colorGreen;
                    psmStatic.startColor = colorGreen;
                }
            }
        }
    }
}