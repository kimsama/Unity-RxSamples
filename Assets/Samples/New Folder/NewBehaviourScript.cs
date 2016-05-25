using UnityEngine;
using System.Collections;
using UniRx;

public class NewBehaviourScript : MonoBehaviour 
{

	// Use this for initialization
	void Start () {

        int[] nums = {1, 2, 3, 4, 5};

        Observable.Range(0, 5)
            .Select(i => nums[i])
            .Subscribe( e => {
                Debug.LogFormat("Value: {0}", e);
            });
	}

    void Sample ()
    {
        /*
        int[] monsters = MapEntityManager.Monsters;
        Observable.Range(0, monsters.Length)
            .Select(i => monsters[i])                                           // get monster IDN
            .Select(idn => TableDataManager.Instance.GetCharacterData( idn ))   // get CharacterTableData
            .Subscribe( data => {

                    Resources.LoadAsync<CharacterInfo>(data.AssetPath)
                        .AsAsyncOperationObservable()
                        .Last()
                        .Subscribe( o => {
                                //OnNext
                                CharacterInfo charInfo = o.asset as CharacterInfo;
                                PoolManager.WarmPool(charInfo.characterPrefab, 2);
                            }, 
                            ()=>{
                                //OnComplete
                            }   
                        );
                },
                (ex) => {
                    // OnError
                }
            );
        */ 
    }
	
}
