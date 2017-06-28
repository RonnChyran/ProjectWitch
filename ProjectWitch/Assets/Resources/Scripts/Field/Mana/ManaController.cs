using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Field.Mana
{
    public class ManaController : MonoBehaviour
    {

        //スポーナー
        [SerializeField]
        private SoldierSpawner mSpawnerA = null;
        [SerializeField]
        private SoldierSpawner mSpawnerB = null;
        [SerializeField]
        private MessageSpawner mMesSpawner = null;

        // Use this for initialization
        void Start()
        {
            StartCoroutine(_Process());
        }

        // Update is called once per frame
        void Update()
        {

        }

        private IEnumerator _Process()
        {
            mSpawnerA.SpawnStart();
            yield return new WaitForSeconds(1.0f);

            mMesSpawner.StartSpawn();
            yield return new WaitForSeconds(2.0f);

            mSpawnerA.SpawnStop();
            yield return new WaitForSeconds(1.0f);

            mSpawnerB.SpawnStart();
            mMesSpawner.StopSpawn();
            yield return new WaitForSeconds(3.0f);


            yield return null;
        }
    }
}