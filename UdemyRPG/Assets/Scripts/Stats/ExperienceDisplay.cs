using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        enum XPUseCase{
            XP_BAR,
            XP_TEXT,
            LVL
        }
        [SerializeField] XPUseCase useCase = XPUseCase.XP_TEXT;
        BaseStats stats;
        Experience currentExperience;

        private void Awake()
        {
            stats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
            currentExperience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            float current = currentExperience.GetCurrentExperience();
            float nextLevel = stats.GetExperienceForNextLevel();
            int currentLevel = stats.GetLevel();
            switch (useCase)
            {
                case XPUseCase.XP_BAR:
                    float newPercentage;
                    if(currentLevel == 1){
                        newPercentage = 100*current/nextLevel;
                    }
                    else{
                        float previousXPCap = stats.GetExperienceForGivenLevel(currentLevel-1);
                        newPercentage = 100 * (current-previousXPCap) / (nextLevel-previousXPCap);
                    }
                    GetComponent<RectTransform>().sizeDelta = new Vector2((Mathf.Round(144 * newPercentage/100)), 20);
                break;
                case XPUseCase.XP_TEXT:
                    GetComponent<Text>().text = string.Format("XP:{0:0}", current);
                break;
                case XPUseCase.LVL:
                    GetComponent<Text>().text = ""+currentLevel;
                break;
                default:
                break;
            }
        }
    }
}
