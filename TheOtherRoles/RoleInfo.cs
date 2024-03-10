using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using InnerNet;
using TheOtherRoles.Helper;
using TheOtherRoles.Utilities;
using UnityEngine;
using static TheOtherRoles.TheOtherRoles;

namespace TheOtherRoles;

public class RoleInfo
{
    public static RoleInfo impostor = new("伪装者", Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, "哈哈！白板内鬼~"), "破坏并击杀所有人", RoleId.Impostor);

    public static RoleInfo assassin = new("刺客", Color.red, "艺术就是刺杀！", "生命就是一场豪赌！", RoleId.EvilGuesser, false, true);
    public static RoleInfo godfather = new("教父", Godfather.color, "懂不懂什么叫黑手啊", "干掉船员", RoleId.Godfather);

    public static RoleInfo mafioso = new("小弟", Mafioso.color, "懂不懂什么叫黑手啊", "帮助<color=#FF1919FF>教父</color>干掉船员", RoleId.Mafioso);

    public static RoleInfo janitor = new("清洁工", Janitor.color, "你有个双胞胎哥哥叫清理者", "帮助教父清理死尸", RoleId.Janitor);
    public static RoleInfo morphling = new("化形者", Morphling.color, "变换你的外形不被抓到", "变换你的外形", RoleId.Morphling);
    public static RoleInfo bomber2 = new("炸弹狂", Bomber2.color, "给其他玩家送炸弹", "炸死所有船员", RoleId.Bomber2);
    public static RoleInfo poucher = new("入殓师", Poucher.color, "你的人生我从未参与，但我送你最后一程", "调查被击杀者的职业", RoleId.Poucher);
    public static RoleInfo mimic = new("模仿者", Mimic.color, "夺走你的一切，我很抱歉", "夺走第一个被你击杀船员的职业", RoleId.Mimic);

    public static RoleInfo camouflager = new("隐蔽者", Camouflager.color, "让我们来猎杀那些陷入黑暗中的人吧", "隐藏在他人之中", RoleId.Camouflager);

    public static RoleInfo miner = new("管道工", Miner.color, "在飞船上打洞", "制造管道", RoleId.Miner);
    public static RoleInfo eraser = new("抹除者", Eraser.color, "你明明拥有一切的....", "抹去敌人的职业", RoleId.Eraser);
    public static RoleInfo vampire = new("吸血鬼", Vampire.color, "让我吸一口，就亿口", "撕咬敌人延迟击杀", RoleId.Vampire);
    public static RoleInfo cleaner = new("清理者", Cleaner.color, "要留清白在人间...吗", "清理尸体", RoleId.Cleaner);
    public static RoleInfo undertaker = new("送葬者", Undertaker.color, "您需要夺命丧葬一条龙服务吗", "拖拽尸体，掩埋命案", RoleId.Undertaker);
    public static RoleInfo escapist = new("逃逸者", Escapist.color, "拜拜了您嘞", "放置锚点并传送", RoleId.Escapist);
    public static RoleInfo warlock = new("术士", Warlock.color, "给其他玩家下咒击杀第三者", "使用术法击杀第三人", RoleId.Warlock);
    public static RoleInfo trickster = new("骗术师", Trickster.color, "黑夜是咱的伪装", "放置惊吓盒并使黑暗降临", RoleId.Trickster);
    public static RoleInfo bountyHunter = new("赏金猎人", BountyHunter.color, "追捕你的赏金目标", "追捕你的赏金目标", RoleId.BountyHunter);
    public static RoleInfo cultist = new("传教士", Cultist.color, "为了古神的诞生", "招募信徒并杀害所有敌人", RoleId.Cultist);
    public static RoleInfo follower = new("新信徒", Cleaner.color, "为了古神的诞生", "杀死所有敌人", RoleId.Follower, true);
    public static RoleInfo bomber = new("恐怖分子", Bomber.color, "我是个疯子，有医生开的证明", "我会给你们数到3的时间，3!bom!", RoleId.Bomber);
    public static RoleInfo blackmailer = new("勒索者", Blackmailer.color, "嘘——红温警告", "勒索其他玩家使其无法发言", RoleId.Blackmailer);
    public static RoleInfo witch = new("女巫", Witch.color, "那么，代价是什么？", "对敌人下咒", RoleId.Witch);
    public static RoleInfo ninja = new("忍者", Ninja.color, "忍者之道，在于隐忍", "标记目标并闪现击杀", RoleId.Ninja);

    public static RoleInfo amnisiac = new("失忆者", Amnisiac.color, "我是你，那你是谁？", "回忆死者的记忆获取职业", RoleId.Amnisiac, true);
    public static RoleInfo jester = new("小丑", Jester.color, "让别人对你的表演叹为观止吧", "想办法驱逐自己", RoleId.Jester, true);
    public static RoleInfo vulture = new("秃鹫", Vulture.color, "人畜无害，可可爱爱~", "吃鸡腿咯~", RoleId.Vulture, true);
    public static RoleInfo lawyer = new("律师", Lawyer.color, "听说每个律师都有专属夺命人", "为客户辩驳帮助胜利", RoleId.Lawyer, true);
    public static RoleInfo prosecutor = new("处刑者", Lawyer.color, "虚假的处刑人，真正的守护天使", "投出你的放逐目标", RoleId.Prosecutor, true);
    public static RoleInfo pursuer = new("起诉人", Pursuer.color, "活下去！", "活下去！", RoleId.Pursuer, true);

    public static RoleInfo jackal = new("豺狼", Jackal.color, "杀死所有船员和<color=#FF1919FF>内鬼</color>", "杀死所有人", RoleId.Jackal, true);

    public static RoleInfo sidekick = new("跟班", Sidekick.color, "帮助豺狼获得胜利", "帮助豺狼获得胜利", RoleId.Sidekick, true);

    public static RoleInfo swooper = new("隐身人", Swooper.color, "嘿！你的小可爱突然出现啦", "隐身并杀死敌人", RoleId.Swooper, true);

    public static RoleInfo arsonist = new("纵火犯", Arsonist.color, "火焰啊赐予我力量！", "燃烧吧，都给我化成灰烬", RoleId.Arsonist, true);

    public static RoleInfo werewolf = new("月下狼人", Werewolf.color, "狂暴之下，万物凋零", "暴走并杀死其他的玩家", RoleId.Werewolf, true);
    //添加天启
    public static RoleInfo thief = new("身份窃贼", Thief.color, "拿来吧你", "通过击杀或猜测窃取对方职业", RoleId.Thief, true);

    public static RoleInfo juggernaut = new("天启", Juggernaut.color, "吾将送汝等救赎，汝等应心怀感激", "减少CD，杀光所有人", RoleId.Juggernaut, true);

    //添加末日预言家
    public static RoleInfo doomsayer = new("末日预言家", Doomsayer.color, "将人类以善恶来区分这根本就是愚蠢的想法", "观察其他玩家，并在会议时刺杀他们", RoleId.Doomsayer, true);

    public static RoleInfo akujo = new RoleInfo("魅魔", Akujo.color, "你们都是我的翅膀！", "招募真爱和备胎并且活下去！", RoleId.Akujo, true);

    public static RoleInfo crewmate = new("船员", Color.white, "哈哈！白板船员~", "发现并驱逐伪装者", RoleId.Crewmate);

    public static RoleInfo goodGuesser = new("侠客", Guesser.color, "生命就是一场豪赌", "在会议上刺杀坏人", RoleId.NiceGuesser);

    //public static RoleInfo modifierNiceGuesser = new("赌怪", Guesser.color, "生命就是一场豪赌", "在会议上刺杀", RoleId.ModifierNiceGuesser,false, true);

    public static RoleInfo mayor = new("市长", Mayor.color, "我持有一票否决权!!!", "用你的权力帮助船员", RoleId.Mayor);
    public static RoleInfo portalmaker = new("星门缔造者", Portalmaker.color, "以[星]之铭", "筑[星]之门", RoleId.Portalmaker);
    public static RoleInfo engineer = new("工程师", Engineer.color, "没人比我更懂得窃听", "维修飞船", RoleId.Engineer);

    public static RoleInfo privateInvestigator = new("观察者", PrivateInvestigator.color, "查看谁在与他人互动", "卧底在飞船之中", RoleId.PrivateInvestigator);

    public static RoleInfo sheriff = new("警长", Sheriff.color, "<color=#FF1919FF>严禁</color>小脑行为！！！", "毙了伪装者", RoleId.Sheriff);

    public static RoleInfo deputy = new("捕快", Sheriff.color, "逮捕<color=#FF1919FF>伪装者</color>", "逮捕伪装者", RoleId.Deputy);
    public static RoleInfo bodyguard = new("保镖", BodyGuard.color, "用自己的生命保护他人", "用自己的生命保护他人", RoleId.BodyGuard);
    public static RoleInfo lighter = new("执灯人", Lighter.color, "你 是 一 个 电 灯 泡a.a", "你的灯光永不熄灭", RoleId.Lighter);
    public static RoleInfo jumper = new("传送师", Jumper.color, "空！间！错！乱！", "放置锚点并传送", RoleId.Jumper);

    //public static RoleInfo arcanist = new("魔术师", Arcanist.color, "使用魔术干扰伪装者", "使用魔术干扰伪装者", RoleId.Arcanist);

    public static RoleInfo detective = new("侦探", Detective.color, "从脚印之中发现<color=#FF1919FF>伪装者</color>", "检查足迹并检验尸体", RoleId.Detective);

    public static RoleInfo timeMaster = new("时间之主", TimeMaster.color, "知道我刚在时空旅行的时候见到谁了么?", "用你的护盾保护你自己", RoleId.TimeMaster);

    public static RoleInfo veteren = new("老兵", Veteren.color, "没有人比我更懂作案时机", "时刻警惕，反弹！", RoleId.Veteren);
    public static RoleInfo medic = new("法医", Medic.color, "学医救不了太空人", "保护船员", RoleId.Medic);
    public static RoleInfo swapper = new("换票师", Swapper.color, "两！极！反！转！", "交换票数", RoleId.Swapper);
    public static RoleInfo seer = new("灵媒", Seer.color, "我知道谁是诱饵", "感知死亡", RoleId.Seer);
    public static RoleInfo hacker = new("黑客", Hacker.color, "哪有那么简单？", "黑入飞船探查他人踪迹", RoleId.Hacker);
    public static RoleInfo tracker = new("追踪者", Tracker.color, "你的一举一动已经暴露", "跟踪可疑玩家", RoleId.Tracker);

    public static RoleInfo snitch = new("告密者", Snitch.color, "无惧生死，秘密行动", "完成任务揭示内鬼", RoleId.Snitch);

    public static RoleInfo spy = new("卧底", Spy.color, "对不起，我是一个[好]人", "潜伏起来，找出内鬼", RoleId.Spy);

    public static RoleInfo securityGuard = new("保安", SecurityGuard.color, "维护飞船秩序", "封锁管道并安放监控", RoleId.SecurityGuard);

    public static RoleInfo medium = new("通灵师", Medium.color, "这不是封建迷信！", "对灵魂通灵获取信息", RoleId.Medium);
    public static RoleInfo trapper = new("设陷师", Trapper.color, "困于陷阱中...", "放置陷阱获取信息", RoleId.Trapper);
    
    //躲猫猫
    public static RoleInfo hunter = new("猎人", Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, "抓捕"), "找到并击杀所有猎物", RoleId.Impostor);

    public static RoleInfo hunted = new("猎物", Color.white, "躲起来", "躲起来", RoleId.Crewmate);

    public static RoleInfo prop = new("躲藏者", Color.white, "伪装成物品并生存", "伪装成物品并生存", RoleId.Crewmate);

    // Modifier
    public static RoleInfo disperser = new("分散者", Color.red, "大伙！该上路了！", "分散所有人", RoleId.Disperser, false, true);

    public static RoleInfo bloody = new("溅血者", Color.yellow, "以吾之血咒汝之身", "用你的血留下死亡信息", RoleId.Bloody, false, true);

    public static RoleInfo antiTeleport = new("通讯兵", Color.yellow, "线上会议！", "无需回到会议室开会", RoleId.AntiTeleport, false, true);

    public static RoleInfo tiebreaker = new("破平者", Color.yellow, "你说得对...但是，规则就是用来打破的", "打破平局", RoleId.Tiebreaker, false, true);

    public static RoleInfo bait = new("诱饵", Color.yellow, "大奖小奖，能记住就是好奖", "击杀你的人会立即报警", RoleId.Bait, false, true);

    public static RoleInfo sunglasses = new("太阳镜", Color.yellow, "这真是，泰酷辣", "你的视野变得更小", RoleId.Sunglasses, false, true);

    public static RoleInfo torch = new("火炬", Color.yellow, "心中有光，照耀四方", "视野增加，无视熄灯", RoleId.Torch, false, true);

    public static RoleInfo flash = new("闪电侠", Color.yellow, "你拥有更快的移动速度!", "你拥有更快的移动速度!", RoleId.Flash, false, true);

    public static RoleInfo multitasker = new("多线程", Color.yellow, "虽然人不可以三心二意，但是你可以一心二用", "交互界面透明", RoleId.Multitasker, false, true);

    public static RoleInfo lover = new("恋人", Lovers.color, "你坠入了爱河", "你坠入了爱河", RoleId.Lover, false, true);

    public static RoleInfo mini = new("小孩", Color.yellow, "《未成年人保护法》第127条[不，是第133条]", "长大之前没有人能伤害你", RoleId.Mini, false, true);

    public static RoleInfo vip = new("VIP", Color.yellow, "我宣布个事儿", "所有人都知道你的死讯", RoleId.Vip, false, true);

    public static RoleInfo indomitable = new("不屈者", Color.yellow, "无所畏惧，愈战愈勇", "无法被猜测", RoleId.Indomitable, false, true);

    public static RoleInfo slueth = new("掘墓人", Color.yellow, "我才是法医！", "报告可知晓死者职业", RoleId.Slueth, false, true);

    public static RoleInfo cursed = new("反骨", Color.yellow, "你是船员....至少现在是", "被内鬼击杀会变成内鬼", RoleId.Cursed, false, true, true);

    public static RoleInfo invert = new("酒鬼", Color.yellow, "打烊前的最后一杯", "你的移动方向被颠倒了", RoleId.Invert, false, true);

    public static RoleInfo blind = new("胆小鬼", Color.yellow, "外面的世界好可怕……", "无法报告尸体", RoleId.Blind, false, true);

    public static RoleInfo watcher = new("窥视者", Color.yellow, "来来来，我看看你们怎么投票的啊", "你可以知晓所有人的投票情况", RoleId.Watcher, false, true);

    public static RoleInfo radar = new("雷达", Color.yellow, "时刻警惕！", "得知距离最近的玩家位置", RoleId.Radar, false, true);

    public static RoleInfo tunneler = new("管道工程师", Color.yellow, "让我看看这个管道", "完成任务可使用管道", RoleId.Tunneler, false, true);

    public static RoleInfo chameleon = new("变色龙", Color.yellow, "看不见我`看不见我`", "静止时可隐身", RoleId.Chameleon, false, true);

    public static RoleInfo shifter = new("交换师", Color.yellow, "你的能力真好用！马上就是我的了！", "与他人交换职业", RoleId.Shifter, false, true);


    public static List<RoleInfo> allRoleInfos = new()
    {
        impostor,
        assassin,
        godfather,
        mafioso,
        janitor,
        morphling,
        bomber2,
        poucher,
        mimic,
        camouflager,
        miner,
        eraser,
        vampire,
        undertaker,
        escapist,
        warlock,
        trickster,
        bountyHunter,
        cultist,
        cleaner,
        bomber,
        blackmailer,
        witch,
        ninja,

        amnisiac,
        jester,
        vulture,
        lawyer,
        prosecutor,
        pursuer,
        jackal,
        sidekick,
        arsonist,
        werewolf,
        thief,
        swooper,
        //天启
        juggernaut,
        //末日预言家
        doomsayer,
        akujo,

        crewmate,
        goodGuesser,
        mayor,
        portalmaker,
        engineer,
        privateInvestigator,
        sheriff,
        deputy,
        bodyguard,
        lighter,
        jumper,
        detective,
        timeMaster,
        veteren,
        medic,
        swapper,
        seer,
        hacker,
        tracker,
        snitch,
        spy,
        securityGuard,
        medium,
        trapper,
        //魔术师
        //arcanist

        //badGuesser,
        disperser,
        bloody,
        antiTeleport,
        tiebreaker,
        bait,
        sunglasses,
        torch,
        flash,
        multitasker,
        lover,
        mini,
        vip,
        indomitable,
        slueth,
        cursed,
        invert,
        blind,
        watcher,
        radar,
        tunneler,
        chameleon,
        shifter
    };


    private static string ReadmePage = "";
    public Color color;
    public string introDescription;
    public bool isGuessable;
    public bool isImpostor;
    public bool isModifier;
    public bool isNeutral;
    public string name;
    public RoleId roleId;
    public string shortDescription;

    public RoleInfo(string name, Color color, string introDescription, string shortDescription, RoleId roleId,
        bool isNeutral = false, bool isModifier = false, bool isGuessable = false, bool isImpostor = false)
    {
        this.color = color;
        this.name = name;
        this.introDescription = introDescription;
        this.shortDescription = shortDescription;
        this.roleId = roleId;
        this.isNeutral = isNeutral;
        this.isModifier = isModifier;
        this.isGuessable = isGuessable;
        this.isImpostor = isImpostor;
    }

    public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, bool showModifier = true)
    {
        var infos = new List<RoleInfo>();
        if (p == null) return infos;

        // Modifier
        if (showModifier)
        {
            // after dead modifier
            if (!CustomOptionHolder.modifiersAreHidden.getBool() || PlayerControl.LocalPlayer.Data.IsDead ||
                AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Ended)
            {
                if (Bait.bait.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bait);
                if (Bloody.bloody.Any(x => x.PlayerId == p.PlayerId)) infos.Add(bloody);
                if (Vip.vip.Any(x => x.PlayerId == p.PlayerId)) infos.Add(vip);
                if (p == Tiebreaker.tiebreaker) infos.Add(tiebreaker);
                if (p == Indomitable.indomitable) infos.Add(indomitable);
                if (p == Cursed.cursed) infos.Add(cursed);
            }

            if (p == Lovers.lover1 || p == Lovers.lover2) infos.Add(lover);
            if (AntiTeleport.antiTeleport.Any(x => x.PlayerId == p.PlayerId)) infos.Add(antiTeleport);
            if (Sunglasses.sunglasses.Any(x => x.PlayerId == p.PlayerId)) infos.Add(sunglasses);
            if (Torch.torch.Any(x => x.PlayerId == p.PlayerId)) infos.Add(torch);
            if (Flash.flash.Any(x => x.PlayerId == p.PlayerId)) infos.Add(flash);
            if (Multitasker.multitasker.Any(x => x.PlayerId == p.PlayerId)) infos.Add(multitasker);
            if (p == Mini.mini) infos.Add(mini);
            if (p == Blind.blind) infos.Add(blind);
            if (p == Watcher.watcher) infos.Add(watcher);
            if (p == Radar.radar) infos.Add(radar);
            if (p == Tunneler.tunneler) infos.Add(tunneler);
            if (p == Slueth.slueth) infos.Add(slueth);
            if (p == Disperser.disperser) infos.Add(disperser);
            if (Invert.invert.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
            if (Chameleon.chameleon.Any(x => x.PlayerId == p.PlayerId)) infos.Add(chameleon);
            if (p == Shifter.shifter) infos.Add(shifter);
            if (Guesser.evilGuesser.Any(x => x.PlayerId == p.PlayerId)) infos.Add(assassin);
        }

        var count = infos.Count; // Save count after modifiers are added so that the role count can be checked

        // Special roles
        if (p == Jester.jester) infos.Add(jester);
        if (p == Swooper.swooper) infos.Add(swooper);
        if (p == Werewolf.werewolf) infos.Add(werewolf);
        if (p == Mayor.mayor) infos.Add(mayor);
        if (p == Portalmaker.portalmaker) infos.Add(portalmaker);
        if (p == Engineer.engineer) infos.Add(engineer);
        if (p == Sheriff.sheriff || p == Sheriff.formerSheriff) infos.Add(sheriff);
        if (p == Deputy.deputy) infos.Add(deputy);
        if (p == Lighter.lighter) infos.Add(lighter);
        if (p == Godfather.godfather) infos.Add(godfather);
        if (p == Miner.miner) infos.Add(miner);
        if (p == Mafioso.mafioso) infos.Add(mafioso);
        if (p == Janitor.janitor) infos.Add(janitor);
        if (p == Morphling.morphling) infos.Add(morphling);
        if (p == Bomber2.bomber2) infos.Add(bomber2);
        if (p == Camouflager.camouflager) infos.Add(camouflager);
        if (p == Vampire.vampire) infos.Add(vampire);
        if (p == Eraser.eraser) infos.Add(eraser);
        if (p == Trickster.trickster) infos.Add(trickster);
        if (p == Cleaner.cleaner) infos.Add(cleaner);
        if (p == Undertaker.undertaker) infos.Add(undertaker);
        if (p == PrivateInvestigator.privateInvestigator) infos.Add(privateInvestigator);
        if (p == Poucher.poucher) infos.Add(poucher);
        if (p == Mimic.mimic) infos.Add(mimic);
        if (p == Warlock.warlock) infos.Add(warlock);
        if (p == Witch.witch) infos.Add(witch);
        if (p == Escapist.escapist) infos.Add(escapist);
        if (p == Ninja.ninja) infos.Add(ninja);
        if (p == Blackmailer.blackmailer) infos.Add(blackmailer);
        if (p == Bomber.bomber) infos.Add(bomber);
        if (p == Detective.detective) infos.Add(detective);
        if (p == TimeMaster.timeMaster) infos.Add(timeMaster);
        if (p == Cultist.cultist) infos.Add(cultist);
        if (p == Amnisiac.amnisiac) infos.Add(amnisiac);
        if (p == Veteren.veteren) infos.Add(veteren);
        if (p == Medic.medic) infos.Add(medic);
        if (p == Swapper.swapper) infos.Add(swapper);
        if (p == BodyGuard.bodyguard) infos.Add(bodyguard);
        if (p == Seer.seer) infos.Add(seer);
        if (p == Hacker.hacker) infos.Add(hacker);
        if (p == Tracker.tracker) infos.Add(tracker);
        if (p == Snitch.snitch) infos.Add(snitch);
        if (p == Jackal.jackal ||
            (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId)))
            infos.Add(jackal);
        if (p == Sidekick.sidekick) infos.Add(sidekick);
        if (p == Follower.follower) infos.Add(follower);
        if (p == Spy.spy) infos.Add(spy);
        if (p == SecurityGuard.securityGuard) infos.Add(securityGuard);
        if (p == Arsonist.arsonist) infos.Add(arsonist);
        if (p == Guesser.niceGuesser) infos.Add(goodGuesser);
        //if (p == Guesser.evilGuesser) infos.Add(badGuesser);
        if (p == BountyHunter.bountyHunter) infos.Add(bountyHunter);
        if (p == Vulture.vulture) infos.Add(vulture);
        if (p == Medium.medium) infos.Add(medium);
        if (p == Lawyer.lawyer && !Lawyer.isProsecutor) infos.Add(lawyer);
        if (p == Lawyer.lawyer && Lawyer.isProsecutor) infos.Add(prosecutor);
        if (p == Trapper.trapper) infos.Add(trapper);
        if (p == Pursuer.pursuer) infos.Add(pursuer);
        if (p == Jumper.jumper) infos.Add(jumper);
        if (p == Thief.thief) infos.Add(thief);
        //天启
        if (p == Juggernaut.juggernaut) infos.Add(juggernaut);
        if (p == Doomsayer.doomsayer) infos.Add(doomsayer);
        if (p == Akujo.akujo) infos.Add(akujo);

        // Default roles (just impostor, just crewmate, or hunter / hunted for hide n seek, prop hunt prop ...
        if (infos.Count == count)
        {
            if (p.Data.Role.IsImpostor)
                infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek ||
                          TORMapOptions.gameMode == CustomGamemodes.PropHunt
                    ? hunter
                    : impostor);
            else
                infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek ? hunted :
                    TORMapOptions.gameMode == CustomGamemodes.PropHunt ? prop : crewmate);
        }

        return infos;
    }

    public static string GetRolesString(PlayerControl p, bool useColors, bool showModifier = true,
        bool suppressGhostInfo = false)
    {
        string roleName;
        roleName = string.Join(" ",
            getRoleInfoForPlayer(p, showModifier).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name)
                .ToArray());
        if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId &&
            CachedPlayer.LocalPlayer.PlayerControl != Lawyer.target)
            roleName += useColors ? Helpers.cs(Pursuer.color, " §") : " §";
        if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(p.PlayerId)) roleName += " (赌怪)";

        if (!suppressGhostInfo && p != null)
        {
            if (p == Shifter.shifter &&
                (CachedPlayer.LocalPlayer.PlayerControl == Shifter.shifter || Helpers.shouldShowGhostInfo()) &&
                Shifter.futureShift != null)
                roleName += Helpers.cs(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
            if (p == Vulture.vulture && (CachedPlayer.LocalPlayer.PlayerControl == Vulture.vulture ||
                                         Helpers.shouldShowGhostInfo()))
                roleName = roleName + Helpers.cs(Vulture.color,
                    $" (剩余 {Vulture.vultureNumberToWin - Vulture.eatenBodies} )");
            if (Helpers.shouldShowGhostInfo())
            {
                if (Eraser.futureErased.Contains(p))
                    roleName = Helpers.cs(Color.gray, "(被抹除) ") + roleName;
                if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                    roleName = Helpers.cs(Vampire.color,
                        $"(被吸血 {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                if (Deputy.handcuffedPlayers.Contains(p.PlayerId))
                    roleName = Helpers.cs(Color.gray, "(被上拷) ") + roleName;
                if (Deputy.handcuffedKnows.ContainsKey(p.PlayerId)) // Active cuff
                    roleName = Helpers.cs(Deputy.color, "(被上拷) ") + roleName;
                if (p == Warlock.curseVictim)
                    roleName = Helpers.cs(Warlock.color, "(被下咒) ") + roleName;
                if (p == Ninja.ninjaMarked)
                    roleName = Helpers.cs(Ninja.color, "(被标记) ") + roleName;
                if (Pursuer.blankedList.Contains(p) && !p.Data.IsDead)
                    roleName = Helpers.cs(Pursuer.color, "(被塞空包弹) ") + roleName;
                if (Witch.futureSpelled.Contains(p) && !MeetingHud.Instance) // This is already displayed in meetings!
                    roleName = Helpers.cs(Witch.color, "☆ ") + roleName;
                if (BountyHunter.bounty == p)
                    roleName = Helpers.cs(BountyHunter.color, "(被悬赏) ") + roleName;
                if (Arsonist.dousedPlayers.Contains(p))
                    roleName = Helpers.cs(Arsonist.color, "♨ ") + roleName;
                if (p == Arsonist.arsonist)
                    roleName = roleName + Helpers.cs(Arsonist.color,
                        $" (剩余 {CachedPlayer.AllPlayers.Count(x => { return x.PlayerControl != Arsonist.arsonist && !x.Data.IsDead && !x.Data.Disconnected && !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); })} )");
                if (p == Jackal.fakeSidekick)
                    roleName = Helpers.cs(Sidekick.color, " (假跟班) ") + roleName;
                if (Akujo.keeps.Contains(p))
                    roleName = Helpers.cs(Color.gray, "(备胎) ") + roleName;
                if (p == Akujo.honmei)
                    roleName = Helpers.cs(Akujo.color, "(真爱) ") + roleName;

                /*
                if ((p == Swooper.swooper) && Jackal.canSwoop2)
                    roleName = Helpers.cs(Swooper.color, $" (Swooper) ") + roleName;
                */

                // Death Reason on Ghosts
                if (p.Data.IsDead)
                {
                    var deathReasonString = "";
                    var deadPlayer = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == p.PlayerId);

                    Color killerColor = new();
                    if (deadPlayer != null && deadPlayer.killerIfExisting != null)
                        killerColor = getRoleInfoForPlayer(deadPlayer.killerIfExisting, false).FirstOrDefault().color;

                    if (deadPlayer != null)
                    {
                        switch (deadPlayer.deathReason)
                        {
                            case DeadPlayer.CustomDeathReason.Disconnect:
                                deathReasonString = " - 断开连接";
                                break;
                            case DeadPlayer.CustomDeathReason.Exile:
                                deathReasonString = " - 被驱逐";
                                break;
                            case DeadPlayer.CustomDeathReason.Kill:
                                deathReasonString =
                                    $" - 被击杀于 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Guess:
                                if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                    deathReasonString = " - 猜测错误";
                                else
                                    deathReasonString =
                                        $" - 被赌杀于 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Shift:
                                deathReasonString =
                                    $" - {Helpers.cs(Color.yellow, "交换")} {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)} 失败";
                                break;
                            case DeadPlayer.CustomDeathReason.WitchExile:
                                deathReasonString =
                                    $" - {Helpers.cs(Witch.color, "被咒杀于")} {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.LoverSuicide:
                                deathReasonString = $" - {Helpers.cs(Lovers.color, "殉情")}";
                                break;
                            case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                deathReasonString = $" - {Helpers.cs(Lawyer.color, "邪恶律师")}";
                                break;
                            case DeadPlayer.CustomDeathReason.Bomb:
                                deathReasonString =
                                    $" - 被恐袭于 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.Arson:
                                deathReasonString =
                                    $" - 被烧死于 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                break;
                            case DeadPlayer.CustomDeathReason.LoveStolen:
                                deathReasonString = 
                                    $" - {Helpers.cs(Lovers.color, "爱人被夺")}";
                                break;
                            case DeadPlayer.CustomDeathReason.Loneliness:
                                deathReasonString = 
                                    $" - {Helpers.cs(Akujo.color, "精力衰竭")}";
                                break;
                        }
                        roleName = roleName + deathReasonString;
                    }
                }
            }
        }

        return roleName;
    }

    public static async Task loadReadme()
    {
        if (ReadmePage == "")
        {
            var client = new HttpClient();
            var response =
                await client.GetAsync(
                    "https://mirror.ghproxy.com/https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherRoles/main/README.md");
            response.EnsureSuccessStatusCode();
            var httpres = await response.Content.ReadAsStringAsync();
            ReadmePage = httpres;
        }
    }

    public static string GetRoleDescription(RoleInfo roleInfo)
    {
        while (ReadmePage == "")
        {
        }

        var index = ReadmePage.IndexOf($"## {roleInfo.name}");
        var endindex = ReadmePage.Substring(index).IndexOf("### Game Options");
        return ReadmePage.Substring(index, endindex);
    }
}