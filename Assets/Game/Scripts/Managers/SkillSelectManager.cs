using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectManager : MonoBehaviour
{
    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
    private static SkillSelectManager _instance;

    // 인스턴스에 접근하기 위한 프로퍼티
    public static SkillSelectManager instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindAnyObjectByType(typeof(SkillSelectManager)) as SkillSelectManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    // 개발용 테스트 플래그 및 테스트 스킬 인덱스
    [SerializeField] private bool isSkillTest;
    [SerializeField] private int testSkillIndex;

    // 최대 스킬 및 패시브 스킬 수 상수 정의
    private const int MAX_SKILL_NUM = 18;
    private const int MAX_PASSIVE_SKILL_NUM = 6;

    // 현재 고를 수 있는 스킬 수와 패시브 스킬 범위 설정
    [SerializeField] private int skillCount = 6;
    [SerializeField] private const int PASSIVESKILLNUM = 100;
    [SerializeField] private int passiveSkillCount = 103;

    // 스킬 선택 버튼
    [SerializeField] private Button skillSelectButton1;
    [SerializeField] private Button skillSelectButton2;
    [SerializeField] private Button skillSelectButton3;

    // 스킬 선택 UI 오브젝트
    [SerializeField] private GameObject skillSelectObject;
    [SerializeField] private GameObject openSkillObject1;
    [SerializeField] private GameObject openSkillObject2;
    [SerializeField] private GameObject closedSkillObject1;
    [SerializeField] private GameObject closedSkillObject2;
    [SerializeField] private GameObject levelUpTextObject;
    [SerializeField] private GameObject[] skillIcon = new GameObject[3];
    [SerializeField] private GameObject[] skillTextName = new GameObject[3];
    [SerializeField] private GameObject[] skillTextDescription = new GameObject[3];
    [SerializeField] private GameObject[] levelObject = new GameObject[3];
    [SerializeField] private List<GameObject> panelSkillIcon = new List<GameObject>(6);
    [SerializeField] private List<GameObject> panelSkillLevelText = new List<GameObject>(6);
    [SerializeField] private GameObject[] panelPassiveSkillIcon = new GameObject[3];
    [SerializeField] private GameObject[] panelPassiveSkillLevelText = new GameObject[3];
    [SerializeField] private GameObject skillPanelObject;

    // 데이터 클래스 참조
    [SerializeField] private SkillData2 skillData;
    [SerializeField] private SkillData2 passiveSkillData;
    [SerializeField] public PlayerData playerData;

    // UI 요소를 캐싱할 변수들
    private Image icon;
    private TextMeshProUGUI textName;
    private TextMeshProUGUI textDescription;

    // 랜덤으로 고를 스킬 인덱스
    private int[] ranNum = new int[3];

    // 현재 상태를 나타내는 변수들
    private bool isChoosingStartSkill;
    private bool isResonateNow;

    // 공명 스킬 관련 변수들
    private int resonanceSkillIndex = -1;
    private int resIndex1 = -1;
    private int resIndex2 = -1;
    private bool isResonanceAlreadyDone;

    private int removedIndex; // 공명 스킬 때문에 삭제된 배열 index
    private GameObject removedPanelSkillIcon;
    private GameObject removedPanelSkillLevelText;

    // 스킬 및 패시브 스킬 최대 레벨 여부를 저장하는 배열
    private bool[] isSkillMaxLevel;
    private bool[] isPassiveSkillMaxLevel;

    // 도트 데미지 스킬들
    private int[] dotDamageSkills = { 2, 7, 8, 11, 12, 13, 14, 15, 16 };

    // 선택된 스킬 및 패시브 스킬 리스트
    private List<int> selectedSkills = new List<int>() { -1, -1, -1, -1, -1, -1 };
    private int selectedSkillsPointer = 0;
    private int[] selectedPassiveSkills = new int[] { -1, -1, -1 };
    private int selectedPassiveSkillsPointer = 0;

    // 금지된 스킬 리스트
    private List<int> bannedSkills = new List<int>();

    // 모든 스킬이 선택되었는지 및 만렙인지 판단하는 변수들
    private bool isSkillAllSelected;
    private bool isPassiveSkillAllSelected;
    private bool isSkillAllMax;

    // 스킬 레벨업 시 계수
    private float normalDamageCoefficient = 1.15f;
    private float normalDelayCoefficient = 0.95f;
    private float normalScaleCoefficient = 1.25f;
    private float maxDamageCoefficient = 1.3f;
    private float maxDelayCoefficient = 0.8f;
    private float maxScaleCoefficient = 1.5f;
    
    // 패시브 스킬 초기값 및 증가값
    private float masterySkillStartValue = 1.2f;
    private float damageReductionSkillStartValue = 0.9f;
    private float speedUpSkillStartValue = 1.15f;
    private float magnetSkillStartValue = 0.5f;

    private float masterySkillIncrementValue = 0.2f;
    private float damageReductionSkillIncrementValue = 0.1f;
    private float speedUpSkillIncrementValue = 0.15f;
    private float magnetSkillIncrementValue = 0.5f;
    
    // 이벤트 델리게이트 정의
    public delegate void OnSkillSelectObjectDisplayed();
    public OnSkillSelectObjectDisplayed onSkillSelectObjectDisplayed;
    public delegate void OnSkillSelectObjectHided();
    public OnSkillSelectObjectDisplayed onSkillSelectObjectHided;
    public delegate void OnPlayerHealed();
    public OnPlayerHealed onPlayerHealed;
    public delegate void OnSkillSelected(int num);
    public OnSkillSelected onSkillSelected;
    public delegate void OnPassiveSkillSelected(int num, float value);
    public OnPassiveSkillSelected onPassiveSkillSelected;
    
    // Awake: 초기 UI 설정
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        // 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않게 함
        DontDestroyOnLoad(gameObject);

        Init();
    }
    
    // Start: 버튼 이벤트 리스너 설정 및 초기화
    private void Start()
    {
        skillSelectButton1.onClick.AddListener(SkillSelectButton1Clicked);
        skillSelectButton2.onClick.AddListener(SkillSelectButton2Clicked);
        skillSelectButton3.onClick.AddListener(SkillSelectButton3Clicked);
    }

    public void Init()
    {
        skillCount = 6;
        passiveSkillCount = 103;

        skillSelectObject.SetActive(false);
        openSkillObject1.SetActive(true);
        openSkillObject2.SetActive(true);
        closedSkillObject1.SetActive(false);
        closedSkillObject2.SetActive(false);
        foreach (var icon in panelSkillIcon) icon.SetActive(false);
        foreach (var icon in panelPassiveSkillIcon) icon.SetActive(false);
        ranNum = new int[3];

        // 현재 상태를 나타내는 변수들
        isChoosingStartSkill = false;
        isResonateNow = false;

        // 공명 스킬 관련 변수들
        resonanceSkillIndex = -1;
        resIndex1 = -1;
        resIndex2 = -1;

        // 스킬 및 패시브 스킬 최대 레벨 여부를 저장하는 배열
        isSkillMaxLevel = new bool[MAX_SKILL_NUM];
        isPassiveSkillMaxLevel = new bool[MAX_PASSIVE_SKILL_NUM];

        // 선택된 스킬 및 패시브 스킬 리스트
        selectedSkills = new List<int>() { -1, -1, -1, -1, -1, -1 };
        selectedSkillsPointer = 0;
        selectedPassiveSkills = new int[] { -1, -1, -1 };
        selectedPassiveSkillsPointer = 0;

        // 금지된 스킬 리스트
        bannedSkills.Clear();

        // 모든 스킬이 선택되었는지 및 만렙인지 판단하는 변수들
        isSkillAllSelected = false;
        isPassiveSkillAllSelected = false;
        isSkillAllMax = false;

        if (isResonanceAlreadyDone)
        {
            // 공명으로 인한 스킬 패널 복구
            RectTransform skillPanelRect = skillPanelObject.GetComponent<RectTransform>();
            skillPanelRect.sizeDelta = new Vector2(skillPanelRect.sizeDelta.x / 5 * 6, skillPanelRect.sizeDelta.y);

            panelSkillIcon.Insert(removedIndex, removedPanelSkillIcon);
            panelSkillLevelText.Insert(removedIndex, removedPanelSkillLevelText);
        }
        isResonanceAlreadyDone = false;

        for (int i = 0; i < panelSkillLevelText.Count; i++)
        {
            textName = panelSkillLevelText[i].GetComponent<TextMeshProUGUI>();
            textName.text = "Lv 111";
        }

        for (int i = 0; i < panelPassiveSkillLevelText.Length; i++)
        {
            textName = panelSkillLevelText[i].GetComponent<TextMeshProUGUI>();
            textName.text = "Lv 111";
        }
    }


    // 시작 스킬 선택
    public void ChooseStartSkill()
    {
        Time.timeScale = 0;
        onSkillSelectObjectDisplayed?.Invoke();
        skillSelectObject.SetActive(true);
        isChoosingStartSkill = true;
        levelUpTextObject.GetComponent<TextMeshProUGUI>().text = "시작 스킬을 선택하세요";
        GameManager.instance.isSkillSelectPageOn = true;
        if (isSkillTest)
        {
            SetSkillPanelForTest();
            return;
        }
        for (int i = 0; i < 3; i++)
        {
            icon = skillIcon[i].GetComponent<Image>();
            icon.sprite = skillData.skillicon[i];
            string color = (i % 3) switch
            {
                0 => "#FF0000",
                1 => "#D2F7FF",
                _ => "#0000FF"
            };
            textName = skillTextName[i].GetComponent<TextMeshProUGUI>();
            textName.text = $"<color={color}>{skillData.skillName[i]}</color>";
            textDescription = skillTextDescription[i].GetComponent<TextMeshProUGUI>();

            String description = "";

            if (Array.IndexOf(dotDamageSkills, i) != -1)
                description = "도트 데미지: ";
            else description = "데미지: ";

            description += skillData.Damage[i];

            description += "\n쿨타임: " + skillData.Delay[i] + "초";

            textDescription.text = $"<color={color}>{description}</color>";
            SetLevelObjectAlpha(i, skillData.level[i]);
        }
    }
    
    // 테스트 스킬 패널 설정
    private void SetSkillPanelForTest()
    {
        int i = 2;
        SetSkillIcon(i, testSkillIndex);
        for (i = 0; i < 2; i++)
        {
            SetSkillIcon(i, i);
        }
    }
    
    // 스킬 아이콘 및 설명 설정
    private void SetSkillIcon(int panelIndex, int skillIndex)
    {
        icon = skillIcon[panelIndex].GetComponent<Image>();
        icon.sprite = skillData.skillicon[skillIndex];
        string color = (panelIndex % 3) switch
        {
            0 => "#FF0000",
            1 => "#D2F7FF",
            _ => "#0000FF"
        };
        textName = skillTextName[panelIndex].GetComponent<TextMeshProUGUI>();
        textName.text = $"<color={color}>{skillData.skillName[skillIndex]}</color>";
        textDescription = skillTextDescription[panelIndex].GetComponent<TextMeshProUGUI>();

        String description = "";

        if (Array.IndexOf(dotDamageSkills, skillIndex) != -1)
            description = "도트 데미지: ";
        else description = "데미지: ";

        if (skillIndex == 12) description += 60;
        else description += skillData.Damage[skillIndex];

        description += "\n쿨타임: " + skillData.Delay[skillIndex] + "초";
        
        textDescription.text = $"<color={color}>{description}</color>";
        Image[] img = levelObject[panelIndex].GetComponentsInChildren<Image>();
        for (int num = 4 - skillData.level[skillIndex]; num >= 0; num--)
        {
            var col = img[num].color;
            col.a = 0.3f;
            img[num].color = col;
        }
        for (int num = 0; num < skillData.level[skillIndex]; num++)
        {
            var col = img[num].color;
            col.a = 1f;
            img[num].color = col;
        }
    }
    
    // 레벨업 패널 표시
    public void DisplayLevelupPanel()
    {
        Time.timeScale = 0;
        CheckResonanceConditions();
        if (resIndex1 > 0 && resIndex2 > 0 && !isResonanceAlreadyDone)
        {
            isResonateNow = true;
            DisplayResonance();
        }
        else
        {
            DisplayNormalWay();
        }
    }
    
    // 공명 조건 검사
    private void CheckResonanceConditions()
    {
        if (selectedSkills[1] < 0 || isResonanceAlreadyDone) return;
        for (int i = 0; i < selectedSkills.Count; i++)
        {
            if (selectedSkills[i] == -1 || !isSkillMaxLevel[selectedSkills[i]]) continue;
            for (int j = i + 1; j < selectedSkills.Count; j++)
            {
                if (selectedSkills[j] == -1 || !isSkillMaxLevel[selectedSkills[j]]) continue;
                if (IsResonate(selectedSkills[i], selectedSkills[j]))
                {
                    resIndex1 = selectedSkills[i];
                    resIndex2 = selectedSkills[j];
                    return;
                }
            }
        }
    }
    
    // 스킬 공명 여부 확인
    private bool IsResonate(int skillIndex1, int skillIndex2)
    {
        resonanceSkillIndex = skillIndex1 switch
        {
            6 when skillIndex2 == 8 => 12,
            8 when skillIndex2 == 6 => 12,
            7 when skillIndex2 == 9 => 13,
            9 when skillIndex2 == 7 => 13,
            11 when skillIndex2 == 7 => 14,
            7 when skillIndex2 == 11 => 14,
            3 when skillIndex2 == 5 => 15,
            5 when skillIndex2 == 3 => 15,
            10 when skillIndex2 == 6 => 16,
            6 when skillIndex2 == 10 => 16,
            8 when skillIndex2 == 10 => 17,
            10 when skillIndex2 == 8 => 17,
            _ => 0
        };
        return resonanceSkillIndex != 0;
    }
    
    // 일반 레벨업 패널 표시
    private void DisplayNormalWay()
    {
        int playerLevel = playerData.level;
        skillCount = playerLevel switch
        {
            <= 10 => 6,
            <= 20 => 9,
            _ => 12
        };
        passiveSkillCount = playerLevel switch
        {
            <= 15 => 103,
            _ => 106
        };
        onSkillSelectObjectDisplayed?.Invoke();
        skillSelectObject.SetActive(true);
        levelUpTextObject.GetComponent<TextMeshProUGUI>().text = "Level UP!";
        List<int> list = new List<int>();

        if (selectedSkillsPointer >= selectedSkills.Count) isSkillAllSelected = true;
        if (selectedPassiveSkillsPointer >= selectedPassiveSkills.Length) isPassiveSkillAllSelected = true;

        if (!isSkillAllSelected)
        {
            AddNonBannedSkills(list);
        }
        else
        {
            AddSelectedNonMaxLevelSkills(list);
        }
        if (!isPassiveSkillAllSelected)
        {
            AddNonMaxPassiveSkills(list);
        }
        else
        {
            AddSelectedNonMaxLevelPassiveSkills(list);
        }
        DisplaySkillPanel(list);
    }
    
    // 금지되지 않은 스킬 추가
    private void AddNonBannedSkills(List<int> list)
    {
        for (int i = 0; i < skillCount; i++)
        {
            if (bannedSkills.Contains(i) || i >= 12 || isSkillMaxLevel[i]) continue;
            list.Add(i);
        }
    }
    
    // 선택된 만렙이 아닌 스킬 추가
    private void AddSelectedNonMaxLevelSkills(List<int> list)
    {
        foreach (int skill in selectedSkills)
        {
            if (skill >= 12 || isSkillMaxLevel[skill]) continue;
            list.Add(skill);
        }
    }
    
    // 만렙이 아닌 패시브 스킬 추가
    private void AddNonMaxPassiveSkills(List<int> list)
    {
        for (int i = PASSIVESKILLNUM; i < passiveSkillCount; i++)
        {
            if (!isPassiveSkillMaxLevel[i - PASSIVESKILLNUM])
                list.Add(i);
        }
    }
    
    // 선택된 만렙이 아닌 패시브 스킬 추가
    private void AddSelectedNonMaxLevelPassiveSkills(List<int> list)
    {
        foreach (int skill in selectedPassiveSkills)
        {
            if (!isPassiveSkillMaxLevel[skill])
                list.Add(skill + PASSIVESKILLNUM);
        }
    }
    
    // 스킬 패널 표시
    private void DisplaySkillPanel(List<int> list)
    {
        if (list.Count == 0)
        {
            skillSelectObject.SetActive(false);
            onSkillSelectObjectHided?.Invoke();
            Time.timeScale = 1;
            return;
        }
        SetSkillPanels(list.Count);

        // 1 ~ 3 번째 패널 중 선택한 스킬 중에서만 뽑을 패널 선택
        // 악용을 막기 위해 50% 확률로만 적용
        int selectedRanNum = -1;
        if (UnityEngine.Random.Range(0,2) == 1)
        {
            selectedRanNum = UnityEngine.Random.Range(0, 3);
        }

        for (int i = 0; i < ranNum.Length && list.Count != 0; i++)
        {
            int ran = UnityEngine.Random.Range(0, list.Count);
            if (i == 0 && list.Count == 1) // 만렙 안찍은 스킬 1개만 남았을 때는 가운데 패널에서 나오게 함
            {
                ranNum[i + 1] = list[ran];
                SetSkillPanel(i + 1);
            }
            else 
            {
                if (i == selectedRanNum) // 선택된 스킬중에서만 랜덤
                {
                    List<int> listCopy = new List<int>();
                    for (int index = 0; index < skillData.skillSelected.Length; index++)
                    {
                        if (skillData.skillSelected[index] && !isSkillMaxLevel[index])
                            listCopy.Add(index);
                    }
                    for (int index = 0; index < passiveSkillData.skillSelected.Length; index++)
                    {
                        if (passiveSkillData.skillSelected[index] && !isPassiveSkillMaxLevel[index])
                            listCopy.Add(index + 100);
                    }
                    for(int index = 0; index < i; index++)
                    {
                        if (listCopy.Contains(ranNum[index]))
                        {
                            listCopy.Remove(ranNum[index]); // 이미 선택창에 나온 스킬들은 제외
                        }
                    }

                    if (listCopy.Count == 0)
                    {
                        ranNum[i] = list[ran];
                    }
                    else
                    {
                        ran = UnityEngine.Random.Range(0, listCopy.Count);
                        ranNum[i] = listCopy[ran];
                    }
                }
                else
                {
                    ranNum[i] = list[ran];
                }

                SetSkillPanel(i);
            }

            list.Remove(ranNum[i]);
        }
    }
    
    // 스킬 패널 개수에 따른 UI 설정
    private void SetSkillPanels(int count)
    {
        openSkillObject1.SetActive(count >= 2);
        closedSkillObject1.SetActive(count < 2);
        openSkillObject2.SetActive(count >= 3);
        closedSkillObject2.SetActive(count < 3);
    }
    
    // 공명 스킬 패널 표시
    private void DisplayResonance()
    {
        onSkillSelectObjectDisplayed?.Invoke();
        skillSelectObject.SetActive(true);
        string color = (resonanceSkillIndex % 3) switch
        {
            0 => "#FF0000",
            1 => "#D2F7FF",
            _ => "#0000FF"
        };
        levelUpTextObject.GetComponent<TextMeshProUGUI>().text = $"<color={color}>{"두 스킬이 공명하고 있습니다!"}</color>";
        openSkillObject1.SetActive(false);
        openSkillObject2.SetActive(false);
        closedSkillObject1.SetActive(false);
        closedSkillObject2.SetActive(false);
        SetResonanceSkillPanel();
    }
    
    // 스킬 패널 설정
    private void SetSkillPanel(int i)
    {
        if (isSkillAllMax) return;
        bool isPassiveSkill = ranNum[i] >= PASSIVESKILLNUM;
        if (!isPassiveSkill)
        {
            SetActiveSkillPanel(i);
        }
        else
        {
            SetPassiveSkillPanel(i);
        }
    }
    
    // 일반 스킬 패널 설정
    private void SetActiveSkillPanel(int i)
    {
        icon = skillIcon[i].GetComponent<Image>();
        icon.sprite = skillData.skillicon[ranNum[i]];
        string color = (ranNum[i] % 3) switch
        {
            0 => "#FF0000",
            1 => "#D2F7FF",
            _ => "#0000FF"
        };
        textName = skillTextName[i].GetComponent<TextMeshProUGUI>();
        textName.text = $"<color={color}>{skillData.skillName[ranNum[i]]}</color>";
        textDescription = skillTextDescription[i].GetComponent<TextMeshProUGUI>();

        String description = SetActiveSkillDescription(i);

        textDescription.text = $"<color={color}>{description}</color>";
        SetLevelObjectAlpha(i, skillData.level[ranNum[i]]);
    }

    String SetActiveSkillDescription(int i)
    {
        int skillIndex = ranNum[i];
        float damage = skillData.Damage[skillIndex];
        float delay = skillData.Delay[skillIndex];
        float normalDamage = Mathf.Floor(damage * normalDamageCoefficient * 100) / 100;
        float normalDelay = Mathf.Floor(delay * normalDelayCoefficient * 100) / 100;
        bool isSelected = skillData.skillSelected[skillIndex];
        bool isDotDamageSkill = Array.IndexOf(dotDamageSkills, skillIndex) != -1;
        int skillLevel = skillData.level[skillIndex];

        string description = "";

        // Description based on skill type
        if (skillIndex != 5)
        {
            description = isDotDamageSkill ? "도트 데미지: " : "데미지: ";
            description += isSelected ? $"{damage:F2} + {normalDamage - damage:F2}" : $"{damage:F0}";
        }

        // Cooldown description
        description += $"\n쿨타임: {delay:F2}초";
        if (isSelected)
        {
            description += $" - {(delay - normalDelay):F2}초";
        }

        // Skill size increase
        if (skillIndex != 4 || skillIndex != 5)
        {
            if (skillLevel == 2)
            {
                description += $"\n스킬 크기 증가: {(normalScaleCoefficient * 100 - 100):F0}%";
            }
            else if (skillLevel == 4)
            {
                description += $"\n스킬 크기 증가: {(maxScaleCoefficient * 100 - 100):F0}%";
            }
        }
        else if (skillLevel == 4) 
        {
            description += "\n스킬 개수 증가";
        }

        return description ;
    }
    
    // 패시브 스킬 패널 설정
    private void SetPassiveSkillPanel(int i)
    {
        icon = skillIcon[i].GetComponent<Image>();
        icon.sprite = passiveSkillData.skillicon[ranNum[i] - PASSIVESKILLNUM];
        string color = (ranNum[i] - PASSIVESKILLNUM) switch
        {
            0 => "#F7570B",
            1 => "#0EB4FC",
            2 => "#79EDFF",
            _ => "#FFFFFF"
        };
        textName = skillTextName[i].GetComponent<TextMeshProUGUI>();
        textName.text = $"<color={color}>{passiveSkillData.skillName[ranNum[i] - PASSIVESKILLNUM]}</color>";
        textDescription = skillTextDescription[i].GetComponent<TextMeshProUGUI>();

        string description = "";
        int skillIndex = ranNum[i];
        int passiveIndex = skillIndex - PASSIVESKILLNUM;
        float damage = passiveSkillData.Damage[passiveIndex];
        bool isSkillSelected = passiveSkillData.skillSelected[passiveIndex];

        if (isSkillSelected)
        {
            description = GetSelectedSkillDescription(skillIndex, damage);
        }
        else
        {
            description = GetUnselectedSkillDescription(skillIndex);
        }
        textDescription.text = $"<color={color}>{description}</color>";
        SetPassiveSkillLevelObjectAlpha(i, passiveSkillData.level[ranNum[i] - PASSIVESKILLNUM]);
    }

    string GetSelectedSkillDescription(int skillNum, float damage)
    {
        float incrementValue = 0;
        string attributeType = "";

        switch (skillNum)
        {
            case 100:
                incrementValue = masterySkillIncrementValue;
                attributeType = "불 속성 데미지 계수";
                break;

            case 101:
                incrementValue = masterySkillIncrementValue;
                attributeType = "전기 속성 데미지 계수";
                break;

            case 102:
                incrementValue = masterySkillIncrementValue;
                attributeType = "물 속성 데미지 계수";
                break;

            case 103:
                incrementValue = damageReductionSkillIncrementValue;
                attributeType = "데미지 감소";
                break;

            case 104:
                incrementValue = speedUpSkillIncrementValue;
                attributeType = "이동 속도 증가";
                break;

            case 105:
                incrementValue = magnetSkillIncrementValue;
                attributeType = "자석 범위 증가";
                break;
        }

        string calculatedValue = "";
        switch (skillNum)
        {
            case 103:
                calculatedValue = "" + Mathf.FloorToInt((1 - damage) * 100) + " + " + Mathf.FloorToInt(incrementValue * 100);
                break;
            case 105:
                calculatedValue = "" + Mathf.FloorToInt(damage * 100) + " + " + Mathf.FloorToInt(incrementValue * 100);
                break;
            default:
                calculatedValue = "" + (Mathf.FloorToInt(damage * 100) - 100) + " + " + Mathf.FloorToInt(incrementValue * 100);
                break;
        }

        return $"{attributeType}: {calculatedValue}%";
    }

    string GetUnselectedSkillDescription(int skillNum)
    {
        float startValue = 0;
        string attributeType = "";

        switch (skillNum)
        {
            case 100:
            case 101:
            case 102:
                startValue = masterySkillStartValue;
                attributeType = "속성 데미지 계수";
                break;

            case 103:
                startValue = 1 - damageReductionSkillStartValue;
                attributeType = "데미지 감소";
                break;

            case 104:
                startValue = speedUpSkillStartValue;
                attributeType = "이동 속도 증가";
                break;

            case 105:
                startValue = magnetSkillStartValue;
                attributeType = "자석 범위 증가";
                break;
        }

        string calculatedValue = "";
        switch (skillNum)
        {
            case 103:
                calculatedValue = "" + Mathf.FloorToInt((1 - startValue) * 100);
                break;
            case 105:
                calculatedValue = "" + Mathf.FloorToInt((startValue - 0.25f) * 100); // 플레이어 Asborber radius 기본 값 바뀌면 수정 해줘야 함
                break;
            default:
                calculatedValue = "" + (Mathf.FloorToInt(startValue * 100) - 100);
                break;
        }

        return $"{attributeType}: +{calculatedValue}%";
    }

    // 레벨 오브젝트 알파값 설정 (일반 스킬)
    private void SetLevelObjectAlpha(int index, int level, bool isResonance = false)
    {
        Image[] img = levelObject[index].GetComponentsInChildren<Image>();

        if (isResonance)
        {
            for (int num = 0; num < level; num++)
            {
                Color col = img[num].color;
                col.a = 0f;
                img[num].color = col;
            }
            return;
        }

        for (int num = 4; num >= level; num--)
        {
            Color col = img[num].color;
            col.a = 0.3f;
            img[num].color = col;
        }
        for (int num = 0; num < level; num++)
        {
            Color col = img[num].color;
            col.a = 1f;
            img[num].color = col;
        }
    }
    
    // 레벨 오브젝트 알파값 설정 (패시브 스킬)
    private void SetPassiveSkillLevelObjectAlpha(int index, int level)
    {
        Image[] img = levelObject[index].GetComponentsInChildren<Image>();
        for (int num = 0; num < 5; num++)
        {
            var col = img[num].color;
            col.a = 0f;
            img[num].color = col;
        }
        for (int num = 3; num > level; num--)
        {
            var col = img[num].color;
            col.a = 0.3f;
            img[num].color = col;
        }
        for (int num = 1; num <= level; num++)
        {
            var col = img[num].color;
            col.a = 1f;
            img[num].color = col;
        }
    }
    
    // 공명 스킬 패널 설정
    private void SetResonanceSkillPanel()
    {
        icon = skillIcon[1].GetComponent<Image>();
        icon.sprite = skillData.skillicon[resonanceSkillIndex];
        string color = (resonanceSkillIndex % 3) switch
        {
            0 => "#FF0000",
            1 => "#D2F7FF",
            _ => "#0000FF"
        };
        textName = skillTextName[1].GetComponent<TextMeshProUGUI>();
        textName.text = $"<color={color}>{skillData.skillName[resonanceSkillIndex]}</color>";
        textDescription = skillTextDescription[1].GetComponent<TextMeshProUGUI>();

        String description = "";

        if (Array.IndexOf(dotDamageSkills, resonanceSkillIndex) != -1)
            description = "도트 데미지: ";
        else description = "데미지: ";

        if (resonanceSkillIndex == 12) description += 200;
        else description += skillData.Damage[resonanceSkillIndex];

        description += "\n쿨타임: " + skillData.Delay[resonanceSkillIndex] + "초";

        textDescription.text = $"<color={color}>{description}</color>";
        SetLevelObjectAlpha(1, 5, true);
    }
    
    // 첫 번째 스킬 선택 버튼 클릭 핸들러
    private void SkillSelectButton1Clicked()
    {
        HandleSkillSelectButton(0);
    }
    
    // 두 번째 스킬 선택 버튼 클릭 핸들러
    private void SkillSelectButton2Clicked()
    {
        if (isResonateNow && !isResonanceAlreadyDone)
        {
            HandleResonanceSkillSelect();
            return;
        }
        HandleSkillSelectButton(1);
    }
    
    // 세 번째 스킬 선택 버튼 클릭 핸들러
    private void SkillSelectButton3Clicked()
    {
        HandleSkillSelectButton(2, true);
    }
    
    // 스킬 선택 버튼 핸들러
    private void HandleSkillSelectButton(int index, bool isThirdButton = false)
    {
        if (isChoosingStartSkill)
        {
            if (isThirdButton && isSkillTest)
            {
                skillData.skillSelected[testSkillIndex] = true;
                skillData.level[testSkillIndex] = 1;
                SetPanelSkillIcon(selectedSkillsPointer, testSkillIndex);
                UpdateSkillLevelText(selectedSkillsPointer, testSkillIndex);
                selectedSkills[selectedSkillsPointer++] = testSkillIndex;
            }
            else
            {
                skillData.skillSelected[index] = true;
                skillData.level[index] = 1;
                SetPanelSkillIcon(selectedSkillsPointer, index);
                UpdateSkillLevelText(selectedSkillsPointer, index);
                selectedSkills[selectedSkillsPointer++] = index;
            }
            isChoosingStartSkill = false;
        }
        else
        {
            bool isPassiveSkill = ranNum[index] >= PASSIVESKILLNUM;
            if (!isPassiveSkill)
            {
                HandleActiveSkillSelect(index);
            }
            else
            {
                HandlePassiveSkillSelect(index);
            }
        }
        skillSelectObject.SetActive(false);
        onSkillSelectObjectHided?.Invoke();
        Time.timeScale = 1;
        GameManager.instance.isSkillSelectPageOn = true;
    }
    
    // 액티브 스킬 선택 처리
    private void HandleActiveSkillSelect(int index)
    {
        if (!skillData.skillSelected[ranNum[index]])
        {
            skillData.skillSelected[ranNum[index]] = true;
            skillData.level[ranNum[index]] = 1;
            SetPanelSkillIcon(selectedSkillsPointer, ranNum[index]);
            UpdateSkillLevelText(selectedSkillsPointer, ranNum[index]);
            selectedSkills[selectedSkillsPointer++] = ranNum[index];
            onSkillSelected?.Invoke(ranNum[index]);
        }
        else
        {
            IncrementSkillLevel(index);
        }
    }
    
    // 스킬 레벨 증가 처리
    private void IncrementSkillLevel(int index)
    {
        skillData.level[ranNum[index]]++;
        isSkillMaxLevel[ranNum[index]] = skillData.level[ranNum[index]] >= 5;
        int skillListIndex = selectedSkills.FindIndex(i => i == ranNum[index]);
        UpdateSkillLevelText(skillListIndex, ranNum[index]);
        if (isSkillMaxLevel[ranNum[index]])
        {
            UpdateSkillStats(ranNum[index], maxDamageCoefficient, maxDelayCoefficient, maxScaleCoefficient);
            textName.text = "Lv Max";
        }
        else if (skillData.level[ranNum[index]] == 3)
        {
            UpdateSkillStats(ranNum[index], normalDamageCoefficient, normalDelayCoefficient, normalScaleCoefficient);
        }
        else
        {
            UpdateSkillStats(ranNum[index], normalDamageCoefficient, normalDelayCoefficient);
        }
        onSkillSelected?.Invoke(ranNum[index]);
    }
    
    // 스킬 스탯 업데이트
    private void UpdateSkillStats(int skillIndex, float damageCoefficient, float delayCoefficient, float? scaleCoefficient = null)
    {
        skillData.Damage[skillIndex] *= damageCoefficient;
        skillData.Delay[skillIndex] *= delayCoefficient;
        if (scaleCoefficient.HasValue)
        {
            skillData.scale[skillIndex] /= normalScaleCoefficient;
            skillData.scale[skillIndex] *= scaleCoefficient.Value;
        }
    }
    
    // 패시브 스킬 선택 처리
    private void HandlePassiveSkillSelect(int index)
    {
        int passiveSkillIndex = ranNum[index] - PASSIVESKILLNUM;
        if (!passiveSkillData.skillSelected[passiveSkillIndex])
        {
            passiveSkillData.skillSelected[passiveSkillIndex] = true;
            passiveSkillData.level[passiveSkillIndex] = 1;
            SetPanelPassiveSkillIcon(selectedPassiveSkillsPointer, passiveSkillIndex);
            selectedPassiveSkills[selectedPassiveSkillsPointer++] = passiveSkillIndex;
            if(passiveSkillIndex < 3)
                passiveSkillData.Damage[passiveSkillIndex] = GetPassiveSkillStartValue(passiveSkillIndex);
            else onPassiveSkillSelected?.Invoke(passiveSkillIndex, GetPassiveSkillStartValue(passiveSkillIndex));
        }
        else
        {
            IncrementPassiveSkillLevel(index, passiveSkillIndex);
        }
    }
    
    // 패시브 스킬 레벨 증가 처리
    private void IncrementPassiveSkillLevel(int index, int passiveSkillIndex)
    {
        passiveSkillData.level[passiveSkillIndex]++;
        isPassiveSkillMaxLevel[passiveSkillIndex] = passiveSkillData.level[passiveSkillIndex] == 3;
        int passiveSkillListIndex = Array.IndexOf(selectedPassiveSkills, passiveSkillIndex);
        UpdatePassiveSkillLevelText(passiveSkillListIndex, passiveSkillIndex);
        UpdatePassiveSkillStats(passiveSkillIndex);
        if (isPassiveSkillMaxLevel[passiveSkillIndex]) textName.text = "Lv Max";
        onPassiveSkillSelected?.Invoke(passiveSkillIndex, passiveSkillData.Damage[passiveSkillIndex]);
    }
    
    // 패시브 스킬 수치 업데이트
    private void UpdatePassiveSkillStats(int passiveSkillIndex)
    {
        switch (passiveSkillIndex)
        {
            case 3: passiveSkillData.Damage[passiveSkillIndex] -= damageReductionSkillIncrementValue; break;
            case 4: passiveSkillData.Damage[passiveSkillIndex] += speedUpSkillIncrementValue; break;
            case 5: passiveSkillData.Damage[passiveSkillIndex] += magnetSkillIncrementValue; break;
            default: passiveSkillData.Damage[passiveSkillIndex] += masterySkillIncrementValue; break;
        }
    }
    
    // 공명 스킬 선택 처리
    private void HandleResonanceSkillSelect()
    {
        // 공명하는 두 스킬 하단 선택된 스킬 선택 안한 걸로 처리
        skillData.skillSelected[resIndex1] = false;
        skillData.skillSelected[resIndex2] = false;

        // 패널에서도 지워주기 위해 공명 될 스킬의 list에서의 index를 저장
        int listResIndex1 = selectedSkills.IndexOf(resIndex1);
        int listResIndex2 = selectedSkills.IndexOf(resIndex2);

        // 두 번째 스킬은 없애고, 첫 번째 스킬은 공명 스킬로 덮어쓰기 할 예정
        selectedSkills.Remove(resIndex2); 

        // 공명 된 두 스킬은 다시 획득할 수 없게 함
        bannedSkills.Add(resIndex1);
        bannedSkills.Add(resIndex2);
        
        // 공명 스킬 선택함
        skillData.skillSelected[resonanceSkillIndex] = true;
        skillData.level[resonanceSkillIndex] = 5;

        selectedSkills[listResIndex1] = resonanceSkillIndex;
        selectedSkillsPointer--; // 포인터 위치도 조정해 줌
        isSkillMaxLevel[resonanceSkillIndex] = true;

        DeactivatePanelSkills(listResIndex1, listResIndex2);
        SetPanelSkillIcon(listResIndex1, resonanceSkillIndex);

        textName = panelSkillLevelText[listResIndex1].GetComponent<TextMeshProUGUI>();
        textName.text = $"Lv Max";
        panelSkillLevelText[listResIndex1].SetActive(true);

        RectTransform skillPanelRect = skillPanelObject.GetComponent<RectTransform>();
        skillPanelRect.sizeDelta = new Vector2(skillPanelRect.sizeDelta.x / 6 * 5, skillPanelRect.sizeDelta.y);
        
        onSkillSelected?.Invoke(resonanceSkillIndex);
        isResonateNow = false;
        openSkillObject1.SetActive(true);
        openSkillObject2.SetActive(true);
        isResonanceAlreadyDone = true;
        skillSelectObject.SetActive(false);
        onSkillSelectObjectHided?.Invoke();
        Time.timeScale = 1;
    }
    
    // 패널 스킬 비활성화 처리
    private void DeactivatePanelSkills(int listResIndex1, int listResIndex2)
    {
        panelSkillIcon[listResIndex1].SetActive(false);
        panelSkillIcon[listResIndex2].SetActive(false);

        removedIndex = listResIndex2;
        removedPanelSkillIcon = panelSkillIcon[listResIndex2];
        removedPanelSkillLevelText = panelSkillLevelText[listResIndex2];

        panelSkillIcon.RemoveAt(listResIndex2);
        panelSkillLevelText.RemoveAt(listResIndex2);
    }

    // 패널 스킬 아이콘 설정
    private void SetPanelSkillIcon(int pointer, int skillIndex)
    {
        icon = panelSkillIcon[pointer].GetComponent<Image>();
        icon.sprite = skillData.skillicon[skillIndex];
        panelSkillIcon[pointer].SetActive(true);
    }
    
    // 스킬 레벨 텍스트 업데이트
    private void UpdateSkillLevelText(int index, int skillIndex)
    {
        textName = panelSkillLevelText[index].GetComponent<TextMeshProUGUI>();
        textName.text = $"Lv {skillData.level[skillIndex]}";
    }
    
    // 패널 패시브 스킬 아이콘 설정
    private void SetPanelPassiveSkillIcon(int pointer, int passiveSkillIndex)
    {
        icon = panelPassiveSkillIcon[pointer].GetComponent<Image>();
        icon.sprite = passiveSkillData.skillicon[passiveSkillIndex];
        panelPassiveSkillIcon[pointer].SetActive(true);
    }
    
    // 패시브 스킬 레벨 텍스트 업데이트
    private void UpdatePassiveSkillLevelText(int index, int passiveSkillIndex)
    {
        textName = panelPassiveSkillLevelText[index].GetComponent<TextMeshProUGUI>();
        textName.text = $"Lv {passiveSkillData.level[passiveSkillIndex]}";
    }
    
    // 패시브 스킬 시작 값 가져오기
    private float GetPassiveSkillStartValue(int passiveSkillIndex)
    {
        return passiveSkillIndex switch
        {
            3 => damageReductionSkillStartValue,
            4 => speedUpSkillStartValue,
            5 => magnetSkillStartValue,
            _ => masterySkillStartValue
        };
    }
}

