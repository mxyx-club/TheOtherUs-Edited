namespace TheOtherRoles.Modules;

public class HudGrid : MonoBehaviour
{
    public static HudGrid Instance { get; set; }

    static HudGrid()
    {
        ClassInjector.RegisterTypeInIl2Cpp<HudGrid>();
    }

    public List<Il2CppArgument<HudContent>>[] Contents = [new(), new()];
    //private Transform ButtonsHolder = null!;
    private Transform StaticButtonsHolder = null!;

    public void Awake()
    {
        var buttonParent = HudManager.Instance.UseButton.transform.parent;
        buttonParent.localPosition = Vector3.zero;
        buttonParent.name = "Buttons";
        Destroy(buttonParent.gameObject.GetComponent<GridArrange>());
        Destroy(buttonParent.gameObject.GetComponent<AspectPosition>());
        //ButtonsHolder = buttonParent;
        StaticButtonsHolder = UnityHelper.CreateObject("StaticButtons", buttonParent.parent, new Vector3(0, 0, -90f)).transform;

        HudContent AddVanillaButtons(GameObject obj, int priority)
        {
            var content = obj.AddComponent<HudContent>();
            content.SetPriority(priority);
            content.ActiveFunc = () => obj.activeSelf && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen);
            RegisterContentToRight(content);

            return content;
        }

        AddVanillaButtons(HudManager.Instance.UseButton.gameObject, 1000);
        AddVanillaButtons(HudManager.Instance.PetButton.gameObject, 1000);
        AddVanillaButtons(HudManager.Instance.ReportButton.gameObject, 995);
        AddVanillaButtons(HudManager.Instance.SabotageButton.gameObject, 990);
        AddVanillaButtons(HudManager.Instance.ImpostorVentButton.gameObject, 980);
        AddVanillaButtons(HudManager.Instance.AbilityButton.gameObject, 300);
        AddVanillaButtons(HudManager.Instance.KillButton.gameObject, 500).MarkAsKillButtonContent();

        //ベントボタンにクールダウンテキストを設定
        /*HudManager.Instance.ImpostorVentButton.cooldownTimerText =
            Instantiate(HudManager.Instance.KillButton.cooldownTimerText, HudManager.Instance.ImpostorVentButton.transform);*/
    }

    public void LateUpdate()
    {
        for (var i = 0; i < 2; i++)
        {
            Contents[i].RemoveAll(c => !c.Value);

            Contents[i].Sort((c1, c2) =>
            {
                int num = c2.Value.Priority - c1.Value.Priority;
                if (num == 0) num = c1.Value.SubPriority - c2.Value.SubPriority;
                return num;
            });


            if (Contents[i].Count == 0) continue;

            var killButtonPosArranged = false;

            int row = 0, column = 0;
            foreach (var c in Contents[i])
            {
                if (!c.Value.IsActive) continue;
                if (InMeeting && !c.Value.gameObject.active) continue; //会議中はstaticContents以外除外する

                if (!killButtonPosArranged && c.Value.MarkedAsKillButtonContent)
                {
                    killButtonPosArranged = true;
                    c.Value.CurrentPos = new Vector2(0, 1);
                    continue;
                }

                c.Value.CurrentPos = new Vector2(column, row);

                if (column < 2 && !c.Value.OccupiesLine)
                {
                    column++;
                }
                else
                {
                    row++;
                    column = 0;
                    if (row == 1 && killButtonPosArranged) column = 1;
                }
            }
        }
    }

    public void RegisterContentToLeft(Il2CppArgument<HudContent> content) => RegisterContent(content, true);

    public void RegisterContentToRight(Il2CppArgument<HudContent> content) => RegisterContent(content, false);

    public void RegisterContent(Il2CppArgument<HudContent> content, bool toLeft)
    {
        Contents[toLeft ? 0 : 1].Add(content);
        content.Value.SetSide(toLeft);

        if (content.Value.IsStaticContent) content.Value.transform.SetParent(Instance?.StaticButtonsHolder);
    }

    private int availableSubPriority = 0;
    public int AvailableSubPriority { get { return availableSubPriority++; } }
}

public class HudContent : MonoBehaviour
{
    static HudContent()
    {
        ClassInjector.RegisterTypeInIl2Cpp<HudContent>();
    }

    public Vector2 CurrentPos { get; set; }

    //Priorityの大きいものから配置される
    public int Priority { get => (OccupiesLine ? 20000 : MarkedAsKillButtonContent ? 10000 : 0) + field; private set; }
    public int SubPriority { get; private set; }

    private bool isLeftSide;
    //private bool isDirty = true;
    public bool OccupiesLine;
    public bool IsStaticContent;
    public Func<bool> ActiveFunc;
    public bool IsActive => ActiveFunc?.Invoke() ?? gameObject.activeSelf;
    public Vector3 ToLocalPos
    {
        get
        {
            float zoomFactor = Camera.main.orthographicSize / 3f;
            var pos = new Vector3(
                      ((4.5f * zoomFactor) - CurrentPos.x) * (isLeftSide ? -1 : 1),
                      (-2.3f * zoomFactor) + CurrentPos.y,
                      0f
            );

            var arrangement = Main.ButtonArrangement.Value;
            if (!MeetingHud.Instance && ((arrangement == 1 && isLeftSide) || arrangement == 2)) pos.y += 0.85f;

            return pos;
        }
    }
    public bool MarkedAsKillButtonContent { get; private set; }
    public void MarkAsKillButtonContent(bool mark = true)
    {
        MarkedAsKillButtonContent = mark;
    }
    public HudContent SetPriority(int priority)
    {
        Priority = priority;
        return this;
    }
    public HudContent UpdateSubPriority()
    {
        SubPriority = HudGrid.Instance?.AvailableSubPriority ?? 0;
        return this;
    }

    public void SetSide(bool asLeftSide)
    {
        isLeftSide = asLeftSide;
    }

    public void OnDisable()
    {
        CurrentPos = new Vector2(-1, -1);
        //isDirty = true;
    }

    public void Start()
    {
        CurrentPos = new Vector2(-1, -1);
    }

    public void LateUpdate()
    {
        if (CurrentPos.x < 0) return;
        /*if (isDirty)
        {
            transform.localPosition = ToLocalPos;
            isDirty = false;
        }
        else
        {
            var diff = ToLocalPos - transform.localPosition;
            transform.localPosition += diff * Time.deltaTime * 5.2f;
        }*/
        transform.localPosition = ToLocalPos;
        //isDirty = false;
    }

    public static HudContent InstantiateContent(string name, bool isLeftSide = true, bool occupiesLine = false, bool asKillButtonContent = false, bool isStaticContent = false)
    {
        var obj = UnityHelper.CreateObject<HudContent>(name, HudManager.Instance.KillButton.transform.parent, Vector3.zero);
        obj.OccupiesLine = occupiesLine;
        obj.MarkAsKillButtonContent(asKillButtonContent);
        obj.IsStaticContent = isStaticContent;

        HudGrid.Instance?.RegisterContent(obj, isLeftSide);
        obj.UpdateSubPriority();

        return obj;
    }
}