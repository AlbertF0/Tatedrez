using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ResultScreenCatalog", menuName = "ScriptableObjects/Result Screen Catalog", order = 1)]
public class ResultScreenCatalog : ScriptableObject
{
    [Serializable]
    public struct ResultElements
    {
        public Sprite Background;
        public string Title;
        public string Description;
        public Color TitleColor;
        public Color DescriptionColor;
        public string ButtonText;
    }

    [SerializeField]    
    private ResultElements _win;
    [SerializeField]
    private ResultElements _lose;
    [SerializeField]
    private ResultElements _draw;


    public ResultElements ReturnResult(GameController.Result result)
    {
        switch (result)
        {
            case GameController.Result.Victory:
                return _win;
               
            case GameController.Result.Defeat:
                return _lose;
              
            default:
                return _draw;
              
        }
    }
}
