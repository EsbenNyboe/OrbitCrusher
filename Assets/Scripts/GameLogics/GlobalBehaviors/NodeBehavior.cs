﻿using System.Collections;
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
        }
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

    public void CollisionNodeComet(int nodeIndex)
    {
        nodeColl[nodeIndex] = true;
        pleaseNormalizeNode[nodeIndex] = false;
        collSizingSpeed[nodeIndex] = collGrowSpeed;
        nodeParticleCometColl[nodeIndex].GetComponent<ParticleSystem>().Play();
        //nodeParticleEnergySphereColl[nodeIndex].GetComponent<ParticleSystem>().Play();
        particleSizing[nodeIndex] = true;
    }
    public void CollisionNodeEnergySphere(GameObject node)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i] == node)
            {
                nodeColl[i] = true;
                pleaseNormalizeNode[i] = false;
                collSizingSpeed[i] = collGrowSpeed;
                //nodeParticleCometColl[i].GetComponent<ParticleSystem>().Play();
                nodeParticleEnergySphereColl[i].GetComponent<ParticleSystem>().Play();
                particleSizing[i] = true;
            }
        }
    }
    public void RemoveTargetHighlighting(int targetIndex)
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            Material nodeMat = LevelManager.nodes[i].GetComponent<MeshRenderer>().material;
            nodeMat.color = Color.gray;
            ParticleSystem.MainModule psmain = nodeParticleStatic[i].GetComponent<ParticleSystem>().main;
            psmain.startColor = Color.red;
        }
    }
    public void HighlightNewTarget(int targetIndex)
    {
        Material nodeMat = LevelManager.nodes[targetIndex].GetComponent<MeshRenderer>().material;
        nodeMat.color = Color.green;
        ParticleSystem.MainModule psmain = nodeParticleStatic[targetIndex].GetComponent<ParticleSystem>().main;
        psmain.startColor = Color.green;

        nodeParticleEnergySphereColl[targetIndex].GetComponent<ParticleSystem>().Play();
    }
}