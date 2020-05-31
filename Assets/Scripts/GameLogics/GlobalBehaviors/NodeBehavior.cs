using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class NodeBehavior : MonoBehaviour
{
    GameObject[] nodes;
    public GameObject nodeParticleCometCollPrefab;
    GameObject[] nodeParticleCometColl;
    public GameObject nodeParticleEnergySphereCollPrefab;
    GameObject[] nodeParticleEnergySphereColl;
    public GameObject nodeParticleStaticPrefab;
    GameObject[] nodeParticleStatic;
    public GameObject nodeParticleSpawnAPrefab;
    GameObject[] nodeParticleSpawnA;
    public GameObject nodeParticleSpawnBPrefab;
    GameObject[] nodeParticleSpawnB;
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

    public void SpawnNodes()
    {
        nodes = LevelManager.nodes;
        nodeNormalSize = nodes[0].transform.localScale.x;
        spawning = new bool[nodes.Length];
        nodeColl = new bool[nodes.Length];
        pleaseNormalizeNode = new bool[nodes.Length];
        collSizingSpeed = new float[nodes.Length];
        particleSizing = new bool[nodes.Length];
        particleShrinking = new bool[nodes.Length];
        nodeParticleCometColl = new GameObject[nodes.Length];
        nodeParticleEnergySphereColl = new GameObject[nodes.Length];
        nodeParticleStatic = new GameObject[nodes.Length];
        nodeParticleSpawnA = new GameObject[nodes.Length];
        nodeParticleSpawnB = new GameObject[nodes.Length];
        nodeDriftMovement = new Animator[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i].transform.localScale = new Vector3(0, 0, 0);
            spawning[i] = true;
            nodeParticleStatic[i] = Instantiate(nodeParticleStaticPrefab, nodes[i].transform);
            nodeParticleStatic[i].SetActive(true);
        }
    }
    public void AllAppear(bool appear)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            nodeParticleStatic[i].SetActive(appear);
            nodes[i].GetComponent<MeshRenderer>().enabled = appear;
            nodeDriftMovement[i] = nodes[i].GetComponentInParent<Animator>();
            nodeDriftMovement[i].enabled = false;
            if (appear)
                StartCoroutine(PlayNodeDriftAnimation(i));
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
            nodeParticleSpawnA[i].GetComponent<ParticleSystem>().Play();
            nodeParticleSpawnB[i].GetComponent<ParticleSystem>().Play();
        }
    }
    public void TargetExplode(int nodeIndex)
    {
        nodeParticleSpawnA[nodeIndex].GetComponent<ParticleSystem>().Play();
        nodeParticleSpawnB[nodeIndex].GetComponent<ParticleSystem>().Play();
    }
    public void LoadLevel()
    {
        if (nodes != null) // is this necessary?
        {
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i].GetComponent<ParticleSystem>() == null)
                {
                    nodeParticleCometColl[i] = Instantiate(nodeParticleCometCollPrefab, nodes[i].transform);
                    nodeParticleCometColl[i].SetActive(true);
                    nodeParticleCometColl[i].GetComponent<ParticleSystem>().Stop();

                    nodeParticleEnergySphereColl[i] = Instantiate(nodeParticleEnergySphereCollPrefab, nodes[i].transform);
                    nodeParticleEnergySphereColl[i].SetActive(true);
                    nodeParticleEnergySphereColl[i].GetComponent<ParticleSystem>().Stop();

                    nodeParticleSpawnA[i] = Instantiate(nodeParticleSpawnAPrefab, nodes[i].transform);
                    nodeParticleSpawnA[i].SetActive(true);
                    
                    nodeParticleSpawnB[i] = Instantiate(nodeParticleSpawnBPrefab, nodes[i].transform);
                    nodeParticleSpawnB[i].SetActive(true);
                }
            }
            AllExplode();
            AllAppear(true);
        }
        ParticleSystem.MainModule psmain = nodeParticleStaticPrefab.GetComponent<ParticleSystem>().main;
        particleSizeInitial = psmain.startSize.constant;
        RemoveTargetHighlighting(LevelManager.targetNodes[0]);
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
                        ParticleSystem.MainModule psmain = nodeParticleStatic[i].GetComponent<ParticleSystem>().main;
                        float size = psmain.startSize.constant;
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
                        psmain.startSize = size;
                    }
                }
            }
        }
    }

    public void CollisionNodeCometColor(int nodeIndex, bool target, bool first, bool objCompleted)
    {
        ParticleSystem psComet = nodeParticleCometColl[nodeIndex].GetComponent<ParticleSystem>();
        ParticleSystem.MainModule psmComet = psComet.main;
        
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
    }
    //public void CollisionNodeComet(int nodeIndex)
    //{
    //    nodeColl[nodeIndex] = true;
    //    pleaseNormalizeNode[nodeIndex] = false;
    //    collSizingSpeed[nodeIndex] = collGrowSpeed;
    //    nodeParticleCometColl[nodeIndex].GetComponent<ParticleSystem>().Play();
    //    //nodeParticleEnergySphereColl[nodeIndex].GetComponent<ParticleSystem>().Play();
    //    particleSizing[nodeIndex] = true;
    //}


    public void CollisionNodeEnergySphereColor(GameObject node, bool correct)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] == node)
            {
                ParticleSystem ps = nodeParticleEnergySphereColl[i].GetComponent<ParticleSystem>();
                ParticleSystem.MainModule psm = ps.main;
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
            Material nodeMat = LevelManager.nodes[i].GetComponent<MeshRenderer>().material;
            nodeMat.color = colorGray;
            ParticleSystem.MainModule psmain = nodeParticleStatic[i].GetComponent<ParticleSystem>().main;
            psmain.startColor = colorRed;
        }
    }
    public void HighlightNewTarget(int targetIndex)
    {
        Material nodeMat = LevelManager.nodes[targetIndex].GetComponent<MeshRenderer>().material;
        nodeMat.color = colorYellow;
        ParticleSystem.MainModule psmain = nodeParticleStatic[targetIndex].GetComponent<ParticleSystem>().main;
        psmain.startColor = colorYellow;

        ParticleSystem ps = nodeParticleEnergySphereColl[targetIndex].GetComponent<ParticleSystem>();
        ParticleSystem.MainModule psm = ps.main;
        psm.startColor = particleYellow;
        ps.Play();
        //nodeParticleEnergySphereColl[targetIndex].GetComponent<ParticleSystem>().Play();
    }

    public void HighlightCompletedTarget(int targetIndex)
    {
        Material nodeMat = LevelManager.nodes[targetIndex].GetComponent<MeshRenderer>().material;
        nodeMat.color = colorGreen;
        ParticleSystem.MainModule psmain = nodeParticleStatic[targetIndex].GetComponent<ParticleSystem>().main;
        psmain.startColor = colorGreen;
    }
}