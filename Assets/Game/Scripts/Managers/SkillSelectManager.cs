using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSelectManager: MonoBehaviour
{
    // 개발용 (스킬 테스트용)
    bool isSkillTest = false;
    int testSkillIndex = 8;

    // 현재 고를 수 있는 스킬 번호 (0 ~ 4 레벨: 5번까지 / 5 ~ 9 레벨: 8번까지 / 10레벨 이상 : 11번까지)
    // 12번은 Blood임
    private int skillCount = 6; // 이건 개수라서 5번까지 나오게 할려면 6개임

    // 패시브 스킬을 3개까지 가지고 있을 수 있음
    private int passiveSkillCount = 19; // 패시브 스킬은 13 ~ 18까지 6개

    // 스킬 선택 버튼들
    public UnityEngine.UI.Button SkillSelectButton1;
    public UnityEngine.UI.Button SkillSelectButton2;
    public UnityEngine.UI.Button SkillSelectButton3;

    // skillSelect 오브젝트
    public GameObject skillSelectObject;

    // Open Skill 오브젝트
    public GameObject openSkillObject1; // 왼쪽 열린 스크롤 오브젝트
    public GameObject openSkillObject2; // 오른쪽 열린 스크롤 오브젝트

    // Closed Skill 오브젝트
    public GameObject closedSkillObject1; // 왼쪽 닫힌 스크롤 오브젝트
    public GameObject closedSkillObject2; // 오른쪽 닫힌 스크롤 오브젝트

    // 레벨 업 문구 오브젝트
    public GameObject levelUpTextObject;

    // skill_Icon 오브젝트
    public GameObject[] skill_Icon = new GameObject[3];

    // skill_TextName 오브젝트
    public GameObject[] skill_TextName = new GameObject[3];

    // skill_TextDescription 오브젝트
    public GameObject[] skill_TextDescription = new GameObject[3];

    // level 오브젝트
    public GameObject[] levelObject = new GameObject[3];

    // panel_skill_Icon 오브젝트
    public GameObject[] panel_skill_Icon = new GameObject[6];

    // panel_skill_LevelText 오브젝트
    public GameObject[] panel_skill_LevelText = new GameObject[6];

    // panel_passive_skill_Icon 오브젝트
    public GameObject[] panel_passive_skill_Icon = new GameObject[3];

    // panel_passive_skill_LevelText 오브젝트
    public GameObject[] panel_passive_skill_LevelText = new GameObject[3];

    public SkillData2 skillData; // 스킬 데이터
    public SkillData2 passiveSkillData; // 패시브 스킬 데이터

    // skillSelectObject의 아이콘, 스킬이름, 스킬 설명
    Image icon;
    TextMeshProUGUI textName;
    TextMeshProUGUI textDescription;

    int[] ranNum = new int[3]; // 스킬들 중에서 랜덤으로 고를 숫자

    bool ischoosingStartSkill; // 시작 스킬을 고르는 상황이냐

    bool[] isSkillMaxLevel; // 만렙인 스킬은 true, 아니면 false로 저장하는 배열
    bool[] isPassiveSkillMaxLevel; // 만렙인 패시브 스킬은 true, 아니면 false로 저장하는 배열

    int[] selected_Skills = new int[] { -1, -1, -1, -1, -1, -1 }; // 선택된 스킬 번호가 들어갈 배열
    int selected_Skills_Pointer = 0;

    int[] selected_Passive_Skills = new int[] { -1, -1, -1}; // 선택된 패시브 스킬 번호가 들어갈 배열
    int selected_Passive_Skills_Pointer = 0;

    bool isSkillAllSelected; // 스킬이 전부 선택됐는지 판단하는 변수
    bool isPassiveSkillAllSelected; // 패시브 스킬이 전부 선택됐는지 판단하는 변수

    bool isSkillAllMax; // 스킬이 전부 만렙인지 판단하는 변수

    // 스킬 레벨업시 데미지, 딜레이 증가 계수
    float normalDamageCoefficient = 1.15f;
    float normalDelayCoefficient = 0.95f;
    float normalscaleCoefficient = 1.25f;

    float maxDamageCoefficient = 1.3f;
    float maxDelayCoefficient = 0.8f;
    float maxscaleCoefficient = 1.5f;

    // 패시브 스킬 1레벨 값
    float masterySkill_StartValue = 1.2f;
    float damageReductionSkill_StartValue = 0.9f;
    float speedUpSkill_StartValue = 1.15f;
    float magnetSkill_StartValue = 0.5f;

    // 패시브 스킬 레벨업시 수치 증가 값(뎀감만 감소함)
    float masterySkill_IncrementValue = 0.2f;
    float damageReductionSkill_IncrementValue = 0.1f; // 이건 뎀감 수치 (레벨업 시 1 -> 0.9 -> 0.8 -> 0.7로 감소함)
    float speedUpSkill_IncrementValue = 0.15f;
    float magnetSkill_IncrementValue = 0.5f;

    // GameManager에게 알려주기 위한 delegate들
    public delegate void OnSkillSelectObjectDisplayed();
    public OnSkillSelectObjectDisplayed onSkillSelectObjectDisplayed;

    public delegate void OnSkillSelectObjectHided();
    public OnSkillSelectObjectDisplayed onSkillSelectObjectHided;

    public delegate void OnPlayerHealed();
    public OnPlayerHealed onPlayerHealed;

    public delegate void OnSkillSelected(int num); // 스킬이 선택되면 즉시 쿨타임 초기화를 해주기 위해 만든 delegate
    public OnSkillSelected onSkillSelected;

    public delegate void OnPassiveSkillSelected(int num, float value); // num: 패시브 스킬 종류, value: 바뀐 수치 값
    public OnPassiveSkillSelected onPassiveSkillSelected;

    private void Awake()
    {
        skillSelectObject.SetActive(false);
        closedSkillObject1.SetActive(false);
        closedSkillObject2.SetActive(false);

        for (int i = 0; i < panel_skill_Icon.Length; i++) { panel_skill_Icon[i].SetActive(false); }
        for (int i = 0; i < panel_passive_skill_Icon.Length; i++) { panel_passive_skill_Icon[i].SetActive(false); }
    }

    private void Start()
    {
        // 왼쪽 버튼 눌렀을 때
        UnityEngine.UI.Button skillSelectButton1 = SkillSelectButton1.GetComponent<UnityEngine.UI.Button>();
        skillSelectButton1.onClick.AddListener(SkillSelectButton1Clicked);

        // 가운데 버튼 눌렀을 때
        UnityEngine.UI.Button skillSelectButton2 = SkillSelectButton2.GetComponent<UnityEngine.UI.Button>();
        skillSelectButton2.onClick.AddListener(SkillSelectButton2Clicked);

        // 오른쪽 버튼 눌렀을 때
        UnityEngine.UI.Button skillSelectButton3 = SkillSelectButton3.GetComponent<UnityEngine.UI.Button>();
        skillSelectButton3.onClick.AddListener(SkillSelectButton3Clicked);

        isSkillMaxLevel = new bool[12];
        isPassiveSkillMaxLevel = new bool[6];
    }

    // 시작 스킬 고르기
    public void ChooseStartSkill()
    {
        Time.timeScale = 0;

        onSkillSelectObjectDisplayed();

        skillSelectObject.SetActive(true);

        ischoosingStartSkill = true; // 시작 스킬을 고르는 상황 (버튼 누를 때 영향이 감)

        levelUpTextObject.GetComponent<TextMeshProUGUI>().text = "Choose Start Skill";

        // 개발용 (시작 스킬에 테스트 스킬 추가)
        if (isSkillTest)
        {
            int i = 2;

            icon = skill_Icon[i].GetComponent<Image>();
            icon.sprite = skillData.skillicon[testSkillIndex];

            string color;

            if (i % 3 == 0) { color = "#FF0000"; }
            else if (i % 3 == 1) { color = "#D2F7FF"; }
            else { color = "#0000FF"; }

            textName = skill_TextName[i].GetComponent<TextMeshProUGUI>();
            textName.text = "<color=" + color + ">" + skillData.skillName[testSkillIndex] + "</color>";

            textDescription = skill_TextDescription[i].GetComponent<TextMeshProUGUI>();
            textDescription.text = "<color=" + color + ">" + skillData.skillDescription[testSkillIndex] + "</color>";

            Image[] img = levelObject[i].GetComponentsInChildren<Image>();

            for (int num = 4 - skillData.level[testSkillIndex]; num >= 0; num--)
            {
                UnityEngine.Color col = img[num].color;
                col.a = 0.3f;
                img[num].color = col;
            }
            for (int num = 0; num < skillData.level[testSkillIndex]; num++)
            {
                UnityEngine.Color col = img[num].color;
                col.a = 1f;
                img[num].color = col;
            }

            for (i = 0; i < 2; i++)
            {
                icon = skill_Icon[i].GetComponent<Image>();
                icon.sprite = skillData.skillicon[i];

                if (i % 3 == 0) { color = "#FF0000"; }
                else if (i % 3 == 1) { color = "#D2F7FF"; }
                else { color = "#0000FF"; }

                textName = skill_TextName[i].GetComponent<TextMeshProUGUI>();
                textName.text = "<color=" + color + ">" + skillData.skillName[i] + "</color>";

                textDescription = skill_TextDescription[i].GetComponent<TextMeshProUGUI>();
                textDescription.text = "<color=" + color + ">" + skillData.skillDescription[i] + "</color>";

                img = levelObject[i].GetComponentsInChildren<Image>();

                for (int num = 4 - skillData.level[i]; num >= 0; num--)
                {
                    UnityEngine.Color col = img[num].color;
                    col.a = 0.3f;
                    img[num].color = col;
                }
                for (int num = 0; num < skillData.level[i]; num++)
                {
                    UnityEngine.Color col = img[num].color;
                    col.a = 1f;
                    img[num].color = col;
                }
            }
            return;
        }

        // 스킬 테스트 아니고 평상시
        for (int i = 0; i < 3; i++)
        {
            icon = skill_Icon[i].GetComponent<Image>();
            icon.sprite = skillData.skillicon[i];

            string color;
            
            if (i % 3 == 0) { color = "#FF0000"; }
            else if (i % 3 == 1) { color = "#D2F7FF"; }
            else { color = "#0000FF"; }

            textName = skill_TextName[i].GetComponent<TextMeshProUGUI>();
            textName.text = "<color=" + color + ">" + skillData.skillName[i] + "</color>";

            textDescription = skill_TextDescription[i].GetComponent<TextMeshProUGUI>();
            textDescription.text = "<color=" + color + ">" + skillData.skillDescription[i] + "</color>";

            Image[] img = levelObject[i].GetComponentsInChildren<Image>();

            for (int num = 4 - skillData.level[i]; num >= 0; num--)
            {
                UnityEngine.Color col = img[num].color;
                col.a = 0.3f;
                img[num].color = col;
            }
            for (int num = 0; num < skillData.level[i]; num++)
            {
                UnityEngine.Color col = img[num].color;
                col.a = 1f;
                img[num].color = col;
            }
        }
    }

    public void DisplayLevelupPanel()
    {
        Time.timeScale = 0;

        int playerLevel = GameManager.instance.player.level;

        if (playerLevel <= 5)
        {
            skillCount = 6;
        }
        else if(playerLevel <= 10)
        {
            skillCount = 9;
        }
        else
        {
            skillCount = 12;
        }

        onSkillSelectObjectDisplayed();

        skillSelectObject.SetActive(true);

        levelUpTextObject.GetComponent<TextMeshProUGUI>().text = "Level UP!";

        List<int> list = new List<int>(); // 이 리스트의 숫자들 중에서 랜덤으로 뽑는 것

        if(selected_Skills_Pointer >= 6)
            isSkillAllSelected = true;

        if(selected_Passive_Skills_Pointer >= 3)
            isPassiveSkillAllSelected = true;

        if (!isSkillAllSelected)
        {
            for (int i = 0; i < skillCount; i++) // list에 스킬 추가
            {
                if (!isSkillMaxLevel[i]) // 만렙인 스킬은 등장 X
                    list.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < selected_Skills.Length; i++)
            {
                if (!isSkillMaxLevel[selected_Skills[i]]) // 만렙인 스킬은 등장 X
                    list.Add(selected_Skills[i]);
            }
        }

        if (!isPassiveSkillAllSelected)
        {
            for (int i = 13; i < passiveSkillCount; i++) // list에 패시브 스킬 추가
            {
                if (!isPassiveSkillMaxLevel[i - 13])
                    list.Add(i);
            }
        }
        else
        {
            for (int i = 0; i < selected_Passive_Skills.Length; i++)
            {
                if (!isPassiveSkillMaxLevel[selected_Passive_Skills[i]]) // 만렙인 스킬은 등장 X
                    list.Add(selected_Passive_Skills[i] + 13);
            }
        }

        if(list.Count == 2) // 만렙 안찍은 스킬이 2개면 실행
        {
            openSkillObject2.SetActive(false);
            closedSkillObject2.SetActive(true);

            for (int i = 0; i < 2; i++)
            {
                int ran = UnityEngine.Random.Range(0, list.Count);

                ranNum[i] = list[ran];

                list.RemoveAt(ran);
            }

            for(int i = 0; i < 2; i++)
            {
                SetSkillPanel(i);
            }
        }else if(list.Count == 1) // 만렙 안찍은 스킬이 1개면 실행
        {
            openSkillObject1.SetActive(false);
            closedSkillObject1.SetActive(true);

            ranNum[1] = list[0];

            SetSkillPanel(1);
        }else if(list.Count == 0) // 스킬 전부 만렙 찍으면 체력 회복하게 함
        {
            isSkillAllMax = true;
            SetSkillPanel(1);
        }
        else
        {
            for (int i = 0; i < 3; i++)
            {
                int ran = UnityEngine.Random.Range(0, list.Count);

                ranNum[i] = list[ran];

                list.RemoveAt(ran);
            }

            for (int i = 0; i < 3; i++)
            {
                SetSkillPanel(i);
            }
        }
    }

    private void SetSkillPanel(int i)
    {
        if (isSkillAllMax) // 스킬, 패시브 전부 다 MAX면
        {
            icon = skill_Icon[i].GetComponent<Image>();
            icon.sprite = skillData.skillicon[12];

            string color = "#FF0000";

            textName = skill_TextName[i].GetComponent<TextMeshProUGUI>();
            textName.text = "<color=" + color + ">" + skillData.skillName[12] + "</color>";

            textDescription = skill_TextDescription[i].GetComponent<TextMeshProUGUI>();
            textDescription.text = "<color=" + color + ">" + skillData.skillDescription[12] + "</color>";

            levelObject[i].SetActive(false);

            return;
        }

        bool isPassiveSkill = ranNum[i] > 12; // 현재 패널에 표시할게 패시브 스킬이냐

        if (!isPassiveSkill) // 패시브 스킬이 아니면 (일반 스킬이면)
        {
            icon = skill_Icon[i].GetComponent<Image>();
            icon.sprite = skillData.skillicon[ranNum[i]];

            // 텍스트 색 설정
            string color;

            if (ranNum[i] % 3 == 0) { color = "#FF0000"; }
            else if (ranNum[i] % 3 == 1) { color = "#D2F7FF"; }
            else { color = "#0000FF"; }

            textName = skill_TextName[i].GetComponent<TextMeshProUGUI>();
            textName.text = "<color=" + color + ">" + skillData.skillName[ranNum[i]] + "</color>";

            textDescription = skill_TextDescription[i].GetComponent<TextMeshProUGUI>();
            textDescription.text = "<color=" + color + ">" + skillData.skillDescription[ranNum[i]] + "</color>";

            Image[] img = levelObject[i].GetComponentsInChildren<Image>();

            // 마법진 알파값 설정 (레벨 표시)
            for (int num = 4; num >= skillData.level[ranNum[i]]; num--)
            {
                UnityEngine.Color col = img[num].color;
                col.a = 0.3f;
                img[num].color = col;
            }
            for (int num = 0; num < skillData.level[ranNum[i]]; num++)
            {
                UnityEngine.Color col = img[num].color;
                col.a = 1f;
                img[num].color = col;
            }
        }
        else // 패시브 스킬 이면
        {
            icon = skill_Icon[i].GetComponent<Image>();
            icon.sprite = passiveSkillData.skillicon[ranNum[i] - 13];

            // 텍스트 색 설정
            string color;
            
            if (ranNum[i] - 13 == 0) { color = "#F7570B"; }
            else if (ranNum[i] - 13 == 1) { color = "#0EB4FC"; }
            else if (ranNum[i] - 13 == 2) { color = "#79EDFF"; }
            else { color = "#FFFFFF"; }

            textName = skill_TextName[i].GetComponent<TextMeshProUGUI>();
            textName.text = "<color=" + color + ">" + passiveSkillData.skillName[ranNum[i] - 13] + "</color>";

            textDescription = skill_TextDescription[i].GetComponent<TextMeshProUGUI>();
            textDescription.text = "<color=" + color + ">" + passiveSkillData.skillDescription[ranNum[i] - 13] + "</color>";

            Image[] img = levelObject[i].GetComponentsInChildren<Image>();

            // 마법진 알파값 설정 (레벨 표시)
            UnityEngine.Color col = img[0].color; // 0, 4번은 아에 안보이게(패시브 스킬은 만렙이 3이니까)
            col.a = 0f;
            img[0].color = col;

            col = img[4].color;
            col.a = 0f;
            img[4].color = col;

            for (int num = 3; num > passiveSkillData.level[ranNum[i] - 13]; num--)
            {
                col = img[num].color;
                col.a = 0.3f;
                img[num].color = col;
            }
            for (int num = 1; num <= passiveSkillData.level[ranNum[i] - 13]; num++)
            {
                col = img[num].color;
                col.a = 1f;
                img[num].color = col;
            }
        }
    }

    private void SkillSelectButton1Clicked()
    {
        if (ischoosingStartSkill)
        {
            skillData.skillSelected[0] = true;
            skillData.level[0] = 1;

            icon = panel_skill_Icon[selected_Skills_Pointer].GetComponent<Image>();
            icon.sprite = skillData.skillicon[0];

            textName = panel_skill_LevelText[selected_Skills_Pointer].GetComponent<TextMeshProUGUI>();
            textName.text = "Lv " + skillData.level[0];

            panel_skill_Icon[selected_Skills_Pointer].SetActive(true);

            selected_Skills[selected_Skills_Pointer++] = 0;

            ischoosingStartSkill = false;
        }
        else
        {
            bool isPassiveSkill = ranNum[0] > 12; // 현재 패널에 표시할게 패시브 스킬이냐

            if (!isPassiveSkill) // 패시브 스킬이 아니면 (일반 스킬이면)
            {
                if (!skillData.skillSelected[ranNum[0]])
                {
                    skillData.skillSelected[ranNum[0]] = true;
                    skillData.level[ranNum[0]] = 1;

                    icon = panel_skill_Icon[selected_Skills_Pointer].GetComponent<Image>();
                    icon.sprite = skillData.skillicon[ranNum[0]];

                    textName = panel_skill_LevelText[selected_Skills_Pointer].GetComponent<TextMeshProUGUI>();
                    textName.text = "Lv " + skillData.level[ranNum[0]];

                    panel_skill_Icon[selected_Skills_Pointer].SetActive(true);

                    selected_Skills[selected_Skills_Pointer++] = ranNum[0];

                    if (ranNum[0] != 12) // blood는 빼고
                        onSkillSelected(ranNum[0]); // delegate 호출
                }
                else
                {
                    skillData.level[ranNum[0]]++;

                    isSkillMaxLevel[ranNum[0]] = skillData.level[ranNum[0]] == 5;

                    int index = Array.IndexOf(selected_Skills, ranNum[0]);

                    textName = panel_skill_LevelText[index].GetComponent<TextMeshProUGUI>();
                    textName.text = "Lv " + skillData.level[ranNum[0]];

                    if (isSkillMaxLevel[ranNum[0]])
                    {
                        // 만랩 찍으면 많이 쌔짐
                        skillData.Damage[ranNum[0]] *= maxDamageCoefficient;
                        skillData.Delay[ranNum[0]] *= maxDelayCoefficient;
                        skillData.scale[ranNum[0]] /= normalscaleCoefficient;
                        skillData.scale[ranNum[0]] *= maxscaleCoefficient;
                        textName.text = "Lv Max";
                    }
                    else if (skillData.level[ranNum[0]] == 3)
                    {
                        skillData.Damage[ranNum[0]] *= normalDamageCoefficient;
                        skillData.Delay[ranNum[0]] *= normalDelayCoefficient;
                        skillData.scale[ranNum[0]] *= normalscaleCoefficient;
                    }
                    else
                    {
                        skillData.Damage[ranNum[0]] *= normalDamageCoefficient;
                        skillData.Delay[ranNum[0]] *= normalDelayCoefficient;
                    }

                    if (ranNum[0] != 12) // blood는 빼고
                        onSkillSelected(ranNum[0]); // delegate 호출
                }
            }
            else // 패시브 스킬이면
            {
                if (!passiveSkillData.skillSelected[ranNum[0] - 13])
                {
                    passiveSkillData.skillSelected[ranNum[0] - 13] = true;
                    passiveSkillData.level[ranNum[0] - 13] = 1;

                    icon = panel_passive_skill_Icon[selected_Passive_Skills_Pointer].GetComponent<Image>();
                    icon.sprite = passiveSkillData.skillicon[ranNum[0] - 13];

                    textName = panel_passive_skill_LevelText[selected_Passive_Skills_Pointer].GetComponent<TextMeshProUGUI>();
                    textName.text = "Lv " + passiveSkillData.level[ranNum[0] - 13];

                    switch (ranNum[0] - 13)
                    {
                        case 3: { passiveSkillData.Damage[ranNum[0] - 13] = damageReductionSkill_StartValue; break; }
                        case 4: { passiveSkillData.Damage[ranNum[0] - 13] = speedUpSkill_StartValue; break; }
                        case 5: { passiveSkillData.Damage[ranNum[0] - 13] = magnetSkill_StartValue; break; }
                        default: { passiveSkillData.Damage[ranNum[0] - 13] = masterySkill_StartValue; break; }
                    }

                    panel_passive_skill_Icon[selected_Passive_Skills_Pointer].SetActive(true);

                    selected_Passive_Skills[selected_Passive_Skills_Pointer++] = ranNum[0] - 13;

                    onPassiveSkillSelected(ranNum[0] - 13, passiveSkillData.Damage[ranNum[0] - 13]); // delegate 호출
                }
                else
                {
                    passiveSkillData.level[ranNum[0] - 13]++;

                    isPassiveSkillMaxLevel[ranNum[0] - 13] = passiveSkillData.level[ranNum[0] - 13] == 3;

                    int index = Array.IndexOf(selected_Passive_Skills, ranNum[0] - 13);

                    textName = panel_passive_skill_LevelText[index].GetComponent<TextMeshProUGUI>();
                    textName.text = "Lv " + passiveSkillData.level[ranNum[0] - 13];

                    switch(ranNum[0] - 13)
                    {
                        case 3: { passiveSkillData.Damage[ranNum[0] - 13] -= damageReductionSkill_IncrementValue; break; }
                        case 4: { passiveSkillData.Damage[ranNum[0] - 13] += speedUpSkill_IncrementValue; break; }
                        case 5: { passiveSkillData.Damage[ranNum[0] - 13] += magnetSkill_IncrementValue; break; }
                        default: { passiveSkillData.Damage[ranNum[0] - 13] += masterySkill_IncrementValue; break; }
                    }

                    if (isPassiveSkillMaxLevel[ranNum[0] - 13]) { textName.text = "Lv Max"; }

                    onPassiveSkillSelected(ranNum[0] - 13, passiveSkillData.Damage[ranNum[0] - 13]); // delegate 호출
                }
            }
        }
        skillSelectObject.SetActive(false);

        onSkillSelectObjectHided();

        Time.timeScale = 1;
    }

    private void SkillSelectButton2Clicked()
    {
        if (!isSkillAllMax)
        {
            if (ischoosingStartSkill)
            {
                skillData.skillSelected[1] = true;
                skillData.level[1] = 1;

                icon = panel_skill_Icon[selected_Skills_Pointer].GetComponent<Image>();
                icon.sprite = skillData.skillicon[1];

                textName = panel_skill_LevelText[selected_Skills_Pointer].GetComponent<TextMeshProUGUI>();
                textName.text = "Lv " + skillData.level[1];

                panel_skill_Icon[selected_Skills_Pointer].SetActive(true);

                selected_Skills[selected_Skills_Pointer++] = 1;

                ischoosingStartSkill = false;
            }
            else
            {
                bool isPassiveSkill = ranNum[1] > 12; // 현재 패널에 표시할게 패시브 스킬이냐

                if (!isPassiveSkill) // 패시브 스킬이 아니면 (일반 스킬이면)
                {
                    if (!skillData.skillSelected[ranNum[1]])
                    {
                        skillData.skillSelected[ranNum[1]] = true;
                        skillData.level[ranNum[1]] = 1;

                        icon = panel_skill_Icon[selected_Skills_Pointer].GetComponent<Image>();
                        icon.sprite = skillData.skillicon[ranNum[1]];

                        textName = panel_skill_LevelText[selected_Skills_Pointer].GetComponent<TextMeshProUGUI>();
                        textName.text = "Lv " + skillData.level[ranNum[1]];

                        panel_skill_Icon[selected_Skills_Pointer].SetActive(true);

                        selected_Skills[selected_Skills_Pointer++] = ranNum[1];

                        if (ranNum[0] != 12) // blood는 빼고
                            onSkillSelected(ranNum[1]); // delegate 호출
                    }
                    else
                    {
                        skillData.level[ranNum[1]]++;

                        isSkillMaxLevel[ranNum[1]] = skillData.level[ranNum[1]] == 5;

                        int index = Array.IndexOf(selected_Skills, ranNum[1]);
                        
                        textName = panel_skill_LevelText[index].GetComponent<TextMeshProUGUI>();
                        textName.text = "Lv " + skillData.level[ranNum[1]];

                        if (isSkillMaxLevel[ranNum[1]])
                        {
                            // 만랩 찍으면 많이 쌔짐
                            skillData.Damage[ranNum[1]] *= maxDamageCoefficient;
                            skillData.Delay[ranNum[1]] *= maxDelayCoefficient;
                            skillData.scale[ranNum[1]] /= normalscaleCoefficient;
                            skillData.scale[ranNum[1]] *= maxscaleCoefficient;
                            textName.text = "Lv Max";
                        }
                        else if (skillData.level[ranNum[1]] == 3)
                        {
                            skillData.Damage[ranNum[1]] *= normalDamageCoefficient;
                            skillData.Delay[ranNum[1]] *= normalDelayCoefficient;
                            skillData.scale[ranNum[1]] *= normalscaleCoefficient;
                        }
                        else
                        {
                            skillData.Damage[ranNum[1]] *= normalDamageCoefficient;
                            skillData.Delay[ranNum[1]] *= normalDelayCoefficient;
                        }

                        if (ranNum[0] != 12) // blood는 빼고
                            onSkillSelected(ranNum[1]); // delegate 호출
                    }
                }
                else // 패시브 스킬이면
                {
                    if (!passiveSkillData.skillSelected[ranNum[1] - 13])
                    {
                        passiveSkillData.skillSelected[ranNum[1] - 13] = true;
                        passiveSkillData.level[ranNum[1] - 13] = 1;

                        icon = panel_passive_skill_Icon[selected_Passive_Skills_Pointer].GetComponent<Image>();
                        icon.sprite = passiveSkillData.skillicon[ranNum[1] - 13];

                        textName = panel_passive_skill_LevelText[selected_Passive_Skills_Pointer].GetComponent<TextMeshProUGUI>();
                        textName.text = "Lv " + passiveSkillData.level[ranNum[1] - 13];

                        switch (ranNum[1] - 13)
                        {
                            case 3: { passiveSkillData.Damage[ranNum[1] - 13] = damageReductionSkill_StartValue; break; }
                            case 4: { passiveSkillData.Damage[ranNum[1] - 13] = speedUpSkill_StartValue; break; }
                            case 5: { passiveSkillData.Damage[ranNum[1] - 13] = magnetSkill_StartValue; break; }
                            default: { passiveSkillData.Damage[ranNum[1] - 13] = masterySkill_StartValue; break; }
                        }

                        panel_passive_skill_Icon[selected_Passive_Skills_Pointer].SetActive(true);

                        selected_Passive_Skills[selected_Passive_Skills_Pointer++] = ranNum[1] - 13;

                        onPassiveSkillSelected(ranNum[1] - 13, passiveSkillData.Damage[ranNum[1] - 13]); // delegate 호출
                    }
                    else
                    {
                        passiveSkillData.level[ranNum[1] - 13]++;

                        isPassiveSkillMaxLevel[ranNum[1] - 13] = passiveSkillData.level[ranNum[1] - 13] == 3;

                        int index = Array.IndexOf(selected_Passive_Skills, ranNum[1] - 13);

                        textName = panel_passive_skill_LevelText[index].GetComponent<TextMeshProUGUI>();
                        textName.text = "Lv " + passiveSkillData.level[ranNum[1] - 13];

                        switch (ranNum[1] - 13)
                        {
                            case 3: { passiveSkillData.Damage[ranNum[1] - 13] -= damageReductionSkill_IncrementValue; break; }
                            case 4: { passiveSkillData.Damage[ranNum[1] - 13] += speedUpSkill_IncrementValue; break; }
                            case 5: { passiveSkillData.Damage[ranNum[1] - 13] += magnetSkill_IncrementValue; break; }
                            default: { passiveSkillData.Damage[ranNum[1] - 13] += masterySkill_IncrementValue; break; }
                        }

                        if (isPassiveSkillMaxLevel[ranNum[1] - 13]) { textName.text = "Lv Max"; }

                        onPassiveSkillSelected(ranNum[1] - 13, passiveSkillData.Damage[ranNum[1] - 13]); // delegate 호출
                    }
                }
            }
        }
        else
        {
            onPlayerHealed();
        }
        skillSelectObject.SetActive(false);

        onSkillSelectObjectHided();

        Time.timeScale = 1;
    }

    private void SkillSelectButton3Clicked()
    {
        if (ischoosingStartSkill)
        {
            if (isSkillTest) // 스킬 테스트 중일 때
            {
                skillData.skillSelected[testSkillIndex] = true;
                skillData.level[testSkillIndex] = 1;

                icon = panel_skill_Icon[selected_Skills_Pointer].GetComponent<Image>();
                icon.sprite = skillData.skillicon[testSkillIndex];

                textName = panel_skill_LevelText[selected_Skills_Pointer].GetComponent<TextMeshProUGUI>();
                textName.text = "Lv " + skillData.level[testSkillIndex];

                panel_skill_Icon[selected_Skills_Pointer].SetActive(true);

                selected_Skills[selected_Skills_Pointer++] = testSkillIndex;

                ischoosingStartSkill = false;
            }
            else // 스킬 테스트 중이 아닌 평소일 때
            {
                skillData.skillSelected[2] = true;
                skillData.level[2] = 1;

                icon = panel_skill_Icon[selected_Skills_Pointer].GetComponent<Image>();
                icon.sprite = skillData.skillicon[2];

                textName = panel_skill_LevelText[selected_Skills_Pointer].GetComponent<TextMeshProUGUI>();
                textName.text = "Lv " + skillData.level[2];

                panel_skill_Icon[selected_Skills_Pointer].SetActive(true);

                selected_Skills[selected_Skills_Pointer++] = 2;

                ischoosingStartSkill = false;
            }
        }
        else
        {
            bool isPassiveSkill = ranNum[2] > 12; // 현재 패널에 표시할게 패시브 스킬이냐

            if (!isPassiveSkill) // 패시브 스킬이 아니면 (일반 스킬이면)
            {
                if (!skillData.skillSelected[ranNum[2]])
                {
                    skillData.skillSelected[ranNum[2]] = true;
                    skillData.level[ranNum[2]] = 1;

                    icon = panel_skill_Icon[selected_Skills_Pointer].GetComponent<Image>();
                    icon.sprite = skillData.skillicon[ranNum[2]];

                    textName = panel_skill_LevelText[selected_Skills_Pointer].GetComponent<TextMeshProUGUI>();
                    textName.text = "Lv " + skillData.level[ranNum[2]];

                    panel_skill_Icon[selected_Skills_Pointer].SetActive(true);

                    selected_Skills[selected_Skills_Pointer++] = ranNum[2];

                    if (ranNum[0] != 12) // blood는 빼고
                        onSkillSelected(ranNum[2]); // delegate 호출
                }
                else
                {
                    skillData.level[ranNum[2]]++;

                    isSkillMaxLevel[ranNum[2]] = skillData.level[ranNum[2]] == 5;

                    int index = Array.IndexOf(selected_Skills, ranNum[2]);

                    textName = panel_skill_LevelText[index].GetComponent<TextMeshProUGUI>();
                    textName.text = "Lv " + skillData.level[ranNum[2]];

                    if (isSkillMaxLevel[ranNum[2]])
                    {
                        // 만랩 찍으면 많이 쌔짐
                        skillData.Damage[ranNum[2]] *= maxDamageCoefficient;
                        skillData.Delay[ranNum[2]] *= maxDelayCoefficient;
                        skillData.scale[ranNum[2]] /= normalscaleCoefficient;
                        skillData.scale[ranNum[2]] *= maxscaleCoefficient;
                        textName.text = "Lv Max";
                    }
                    else if (skillData.level[ranNum[2]] == 3)
                    {
                        skillData.Damage[ranNum[2]] *= normalDamageCoefficient;
                        skillData.Delay[ranNum[2]] *= normalDelayCoefficient;
                        skillData.scale[ranNum[2]] *= normalscaleCoefficient;
                    }
                    else
                    {
                        skillData.Damage[ranNum[2]] *= normalDamageCoefficient;
                        skillData.Delay[ranNum[2]] *= normalDelayCoefficient;
                    }

                    if (ranNum[0] != 12) // blood는 빼고
                        onSkillSelected(ranNum[2]); // delegate 호출
                }
            }
            else // 패시브 스킬이면
            {
                if (!passiveSkillData.skillSelected[ranNum[2] - 13])
                {
                    passiveSkillData.skillSelected[ranNum[2] - 13] = true;
                    passiveSkillData.level[ranNum[2] - 13] = 1;

                    icon = panel_passive_skill_Icon[selected_Passive_Skills_Pointer].GetComponent<Image>();
                    icon.sprite = passiveSkillData.skillicon[ranNum[2] - 13];

                    textName = panel_passive_skill_LevelText[selected_Passive_Skills_Pointer].GetComponent<TextMeshProUGUI>();
                    textName.text = "Lv " + passiveSkillData.level[ranNum[2] - 13];

                    switch (ranNum[2] - 13)
                    {
                        case 3: { passiveSkillData.Damage[ranNum[2] - 13] = damageReductionSkill_StartValue; break; }
                        case 4: { passiveSkillData.Damage[ranNum[2] - 13] = speedUpSkill_StartValue; break; }
                        case 5: { passiveSkillData.Damage[ranNum[2] - 13] = magnetSkill_StartValue; break; }
                        default: { passiveSkillData.Damage[ranNum[2] - 13] = masterySkill_StartValue; break; }
                    }

                    panel_passive_skill_Icon[selected_Passive_Skills_Pointer].SetActive(true);

                    selected_Passive_Skills[selected_Passive_Skills_Pointer++] = ranNum[2] - 13;

                    onPassiveSkillSelected(ranNum[2] - 13, passiveSkillData.Damage[ranNum[2] - 13]); // delegate 호출
                }
                else
                {
                    passiveSkillData.level[ranNum[2] - 13]++;

                    isPassiveSkillMaxLevel[ranNum[2] - 13] = passiveSkillData.level[ranNum[2] - 13] == 3;

                    int index = Array.IndexOf(selected_Passive_Skills, ranNum[2] - 13);

                    textName = panel_passive_skill_LevelText[index].GetComponent<TextMeshProUGUI>();
                    textName.text = "Lv " + passiveSkillData.level[ranNum[2] - 13];

                    switch (ranNum[2] - 13)
                    {
                        case 3: { passiveSkillData.Damage[ranNum[2] - 13] -= damageReductionSkill_IncrementValue; break; }
                        case 4: { passiveSkillData.Damage[ranNum[2] - 13] += speedUpSkill_IncrementValue; break; }
                        case 5: { passiveSkillData.Damage[ranNum[2] - 13] += magnetSkill_IncrementValue; break; }
                        default: { passiveSkillData.Damage[ranNum[2] - 13] += masterySkill_IncrementValue; break; }
                    }

                    if (isPassiveSkillMaxLevel[ranNum[2] - 13]) { textName.text = "Lv Max"; }

                    onPassiveSkillSelected(ranNum[2] - 13, passiveSkillData.Damage[ranNum[2] - 13]); // delegate 호출
                }
            }
        }
        skillSelectObject.SetActive(false);

        onSkillSelectObjectHided();

        Time.timeScale = 1;
    }
}

