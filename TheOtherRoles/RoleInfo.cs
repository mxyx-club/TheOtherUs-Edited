using System.Linq;
using System;
using System.Collections.Generic;
using TheOtherRoles.Players;
using static TheOtherRoles.TheOtherRoles;
using UnityEngine;
using TheOtherRoles.Utilities;
using TheOtherRoles.CustomGameModes;
using System.Threading.Tasks;
using System.Net.Http;
using static TheOtherRoles.Guesser;

namespace TheOtherRoles
{
    public class RoleInfo {
        public Color color;
        public string name;
        public string introDescription;
        public string shortDescription;
        public RoleId roleId;
        public bool isNeutral;
        public bool isGuessable;
        public bool isModifier;
        public bool isImpostor;

        public RoleInfo(string name, Color color, string introDescription, string shortDescription, RoleId roleId, bool isNeutral = false, bool isModifier = false, bool isGuessable = false,bool isImpostor = false) {
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


        public static RoleInfo mayor = new RoleInfo("市长", Mayor.color, "安静！都听我说，我的权力很大", "在会议中拥有两票", RoleId.Mayor);
        public static RoleInfo portalmaker = new RoleInfo("星门缔造者", Portalmaker.color, "以[星]之名，筑[星]之门", "筑[星]之门", RoleId.Portalmaker);
        public static RoleInfo engineer = new RoleInfo("工程师", Engineer.color, "没有人比我更懂窃听", "钻洞修理会议暴毙[你的一生]，修理破坏", RoleId.Engineer);
        public static RoleInfo privateInvestigator = new RoleInfo("观察者", PrivateInvestigator.color, "注视难以察觉的细节", "观察他人技能是否交互.", RoleId.PrivateInvestigator);
        public static RoleInfo sheriff = new RoleInfo("警长", Sheriff.color, "谁说AU不能钓鱼", "我的眼里容不得沙子，执法所有坏人", RoleId.Sheriff);
        public static RoleInfo bodyguard = new RoleInfo("保镖", BodyGuard.color, "用生命保护你，我的好homie", "可以用生命保护一位玩家", RoleId.BodyGuard, false);
        public static RoleInfo deputy = new RoleInfo("辅警", Sheriff.color, "谁也逃不过一句[我被赌必是他]", "我真的不是跟班[给玩家上手铐]", RoleId.Deputy);
        public static RoleInfo lighter = new RoleInfo("执灯人", Lighter.color, "光芒！永不熄灭！", "照亮前行的道路", RoleId.Lighter);
        public static RoleInfo goodGuesser = new RoleInfo("赌怪", Guesser.color, "赌上[盯]的一切！盖亚！", "生命就是豪赌[你说是吧，盯]", RoleId.NiceGuesser);
        public static RoleInfo crewmate = new RoleInfo("船员", Color.white, "找出内鬼", "完成任务", RoleId.Crewmate);
        public static RoleInfo jumper = new RoleInfo("传送师", Jumper.color, "空！间！错！乱！", "放置锚点并传送", RoleId.Jumper);
        public static RoleInfo detective = new RoleInfo("侦探", Detective.color, "心机之蛙一直摸你肚子", "调查足迹和凶手", RoleId.Detective);
        public static RoleInfo timeMaster = new RoleInfo("时间之主", TimeMaster.color, "知道我刚在时空旅行的时候见到谁了么?是金色暗影!", "开启时光之盾", RoleId.TimeMaster);
        public static RoleInfo veteren = new RoleInfo("老兵", Veteren.color, "没有人比我更懂作案时机", "时刻警惕，反弹！", RoleId.Veteren);
        public static RoleInfo medic = new RoleInfo("法医", Medic.color, "学医救不了太空人", "保护船员", RoleId.Medic);
        public static RoleInfo swapper = new RoleInfo("换票师", Swapper.color, "两！极！反！转！", "交换票数", RoleId.Swapper);
        public static RoleInfo seer = new RoleInfo("灵媒", Seer.color, "我知道谁是诱饵", "感知死亡", RoleId.Seer);
        public static RoleInfo hacker = new RoleInfo("骇客", Hacker.color, "哪有那么简单？", "骇入飞船探查他人踪迹", RoleId.Hacker);
        public static RoleInfo tracker = new RoleInfo("跟踪者", Tracker.color, "你的一举一动已经暴露", "跟踪可疑玩家", RoleId.Tracker);
        public static RoleInfo snitch = new RoleInfo("告密者", Snitch.color, "无惧生死<color=#FF1919FF>,秘密行动</color>", "完成任务揭示坏人", RoleId.Snitch);
        public static RoleInfo spy = new RoleInfo("卧底", Spy.color, "对不起，我是梁朝伟", "潜伏起来，找出内鬼", RoleId.Spy);
        public static RoleInfo securityGuard = new RoleInfo("保安", SecurityGuard.color, "维护飞船秩序", "封锁管道并安放监控", RoleId.SecurityGuard);
        public static RoleInfo medium = new RoleInfo("通灵师", Medium.color, "天清地灵,众鬼听令！", "对灵魂通灵获取信息", RoleId.Medium);
        public static RoleInfo trapper = new RoleInfo("陷阱师", Trapper.color, "困于陷阱中...", "放置陷阱获取信息", RoleId.Trapper);

        public static RoleInfo godfather = new RoleInfo("黑手党", Godfather.color, "懂不懂什么叫黑手啊", "杀害所有船员", RoleId.Godfather,false, false, false, true);
        public static RoleInfo mafioso = new RoleInfo("小弟", Mafioso.color, "懂不懂什么叫黑手啊", "杀害所有船员", RoleId.Mafioso, false, false, false, true);
        public static RoleInfo janitor = new RoleInfo("清洁工", Janitor.color, "听说我哥叫清理者", "清理死尸", RoleId.Janitor, false, false, false, true);
        public static RoleInfo morphling = new RoleInfo("化形者", Morphling.color, "[H]，我终究是变成了你，去寻了[好人]", "变换你的外形", RoleId.Morphling, false, false, false, true);
        public static RoleInfo bomber2 = new RoleInfo("炸弹人", Bomber2.color, "想要我手中的24K纯金炸弹吗", "将炸弹传给其他人", RoleId.Bomber2, false, false, false, true);
        public static RoleInfo poucher = new RoleInfo("入殓师", Poucher.color, "你的人生我从未参与，但我送你最后一程", "调查被击杀者的职业", RoleId.Poucher, false, false, false, true);
        public static RoleInfo mimic = new RoleInfo("模仿者", Mimic.color, "夺走你的一切，我很抱歉", "夺走第一个被你击杀船员的职业", RoleId.Mimic, false, false, false, true);
        public static RoleInfo camouflager = new RoleInfo("隐蔽者", Camouflager.color, "让我们来猎杀那些陷入黑暗中的人吧", "开启小黑人状态，杀杀杀", RoleId.Camouflager, false, false, true, true);
        public static RoleInfo miner = new RoleInfo("管道工", Miner.color, "再也不想听到[你是来拉粑粑的吧]这句话了~", "制造管道", RoleId.Miner, false, false, false, true);
        public static RoleInfo vampire = new RoleInfo("吸血鬼", Vampire.color, "让我吸一口，就亿口", "撕咬敌人延迟击杀", RoleId.Vampire, false, false, false, true);
        public static RoleInfo eraser = new RoleInfo("抹除者", Eraser.color, "你明明拥有一切的....", "抹去敌人的职业", RoleId.Eraser, false, false, false, true);
        public static RoleInfo trickster = new RoleInfo("骗术师", Trickster.color, "黑夜是咱的伪装", "放置惊吓盒并使黑暗降临", RoleId.Trickster, false, false, false, true);
        public static RoleInfo cleaner = new RoleInfo("清理者", Cleaner.color, "要留清白在人间...吗", "清理尸体", RoleId.Cleaner, false, false, false, true);
        public static RoleInfo undertaker = new RoleInfo("承办丧葬者", Undertaker.color, "您需要夺命丧葬一条龙服务吗", "拖拽尸体，掩埋命案", RoleId.Undertaker, false, false, false, true);
        public static RoleInfo warlock = new RoleInfo("术士", Warlock.color, "人心啊，最容易被利用了[善良的术士]", "使用术法击杀第三人", RoleId.Warlock, false, false, false, true);
        public static RoleInfo bountyHunter = new RoleInfo("赏金猎人", BountyHunter.color, "自信点，你就是下一个龙魂使", "猎杀你的悬赏目标", RoleId.BountyHunter, false, false, false, true);
        public static RoleInfo witch = new RoleInfo("女巫", Witch.color, "那么，代价是什么？", "对敌人下咒", RoleId.Witch, false, false, false, true);
        public static RoleInfo escapist = new RoleInfo("逃逸者", Escapist.color, "拜拜了您嘞", "放置锚点并传送", RoleId.Escapist, false, false, false, true);
        public static RoleInfo cultist = new RoleInfo("传教士", Cultist.color, "为了古神的诞生", "招募信徒并杀害所有敌人", RoleId.Cultist, false, false, false, true);
        public static RoleInfo ninja = new RoleInfo("忍者", Ninja.color, "忍者之道，在于隐忍", "标记忍杀目标并远程击杀", RoleId.Ninja, false, false, false, true);
        public static RoleInfo blackmailer = new RoleInfo("勒索者", Blackmailer.color, "嘘——红温警告`", "勒索其他玩家使其无法发言", RoleId.Blackmailer, false, false, false, true);
        public static RoleInfo bomber = new RoleInfo("恐怖分子", Bomber.color, "我是个疯子，有医生开的证明", "我会给你们数到3的时间，3！bom![放置炸弹]", RoleId.Bomber, false, false, false, true);
        public static RoleInfo impostor = new RoleInfo("内鬼", Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, "击杀与破坏"), "杀害所有敌人", RoleId.Impostor, false, false, false, true);






        //public static RoleInfo jackal = new RoleInfo("Jackal", Jackal.color, "Kill all Crewmates and <color=#FF1919FF>Impostors</color> to win", "Kill everyone", RoleId.Jackal, true);
        public static RoleInfo jackal = new RoleInfo("豺狼", Jackal.color, "听说有个叫[和风]的倒霉蛋，走！尝尝咸蛋！", "招募跟班并杀死所有人", RoleId.Jackal, true);
        public static RoleInfo sidekick = new RoleInfo("跟班", Sidekick.color, "代代相传，为图霸业", "帮助豺狼杀死所有人", RoleId.Sidekick, true);
        public static RoleInfo follower = new RoleInfo("新信徒", Cleaner.color, "为了古神的诞生", "杀死所有敌人", RoleId.Follower, true);
        public static RoleInfo arsonist = new RoleInfo("纵火犯", Arsonist.color, "火焰啊赐予我力量！", "燃烧吧，都给我化成灰烬", RoleId.Arsonist, true);
        public static RoleInfo amnisiac = new RoleInfo("失忆者", Amnisiac.color, "我是你，那你是谁？", "窃取死者记忆并获取对方职业", RoleId.Amnisiac, true);
        //public static RoleInfo badGuesser = new RoleInfo("Evil Guesser", Palette.ImpostorRed, "Guess and shoot", "Guess and shoot", RoleId.EvilGuesser);
        public static RoleInfo vulture = new RoleInfo("秃鹫", Vulture.color, "1234，人畜无害，可可爱爱", "找饭吃", RoleId.Vulture, true);
        public static RoleInfo lawyer = new RoleInfo("辩护律师", Lawyer.color, "听说每个律师都有专属夺命人", "为客户辩驳帮助胜利", RoleId.Lawyer, true);
        public static RoleInfo prosecutor = new RoleInfo("处刑人", Lawyer.color, "虚假的处刑人，真正的守护天使", "把目标投出去", RoleId.Prosecutor, true);
        public static RoleInfo pursuer = new RoleInfo("起诉人", Pursuer.color, "活下去！", "活下去！[Blacks]马上就来了！", RoleId.Pursuer,true);
        public static RoleInfo jester = new RoleInfo("小丑yumu", Jester.color, "成为[雨沐]吧，开香槟喽", "N狼在场怎么输，有我在没意外，努力把自己投出去", RoleId.Jester, true);
        public static RoleInfo werewolf = new RoleInfo("月下狼人", Werewolf.color, "原来我也可以成为[哇哈哈]", "狂暴一开，谁都不爱，杀光所有人", RoleId.Werewolf, true);
        public static RoleInfo thief = new RoleInfo("身份窃贼", Thief.color, "拿来吧你", "通过击杀或猜测窃取对方职业", RoleId.Thief, true);
        //天启添加
        public static RoleInfo juggernaut = new RoleInfo("天启", Juggernaut.color, "吾将送汝等救赎，汝等应心怀感激", "减少CD，杀光所有人", RoleId.Juggernaut, true);
        public static RoleInfo doomsayer = new RoleInfo("末日预言家", Doomsayer.color, "你的死兆星在天上闪耀", "预言出职业，赌死到目标数", RoleId.Doomsayer, true);
        public static RoleInfo hunter = new RoleInfo("猎人", Palette.ImpostorRed, Helpers.cs(Palette.ImpostorRed, "抓捕"), "找到并击杀所有猎物", RoleId.Impostor);
        public static RoleInfo hunted = new RoleInfo("猎物", Color.white, "躲起来", "躲起来", RoleId.Crewmate);
        public static RoleInfo prop = new RoleInfo("躲藏者", Color.white, "伪装成物品并活下去", "伪装成物品并活下去", RoleId.Crewmate);



        // Modifier
        public static RoleInfo bloody = new RoleInfo("溅血者", Color.yellow, "以吾之血咒汝之身", "用你的血留下死亡信息", RoleId.Bloody, false, true);
        public static RoleInfo antiTeleport = new RoleInfo("通讯兵", Color.yellow, "线上会议！", "无需回到会议室开会", RoleId.AntiTeleport, false, true);
        public static RoleInfo tiebreaker = new RoleInfo("破平者", Color.yellow, "你说得对...但是，规则就是用来打破的", "票计1.5票", RoleId.Tiebreaker, false, true);
        public static RoleInfo bait = new RoleInfo("诱饵", Color.yellow, "听说我有个外号叫咸蛋", "击杀你的人会立即报警", RoleId.Bait, false, true);
        public static RoleInfo sunglasses = new RoleInfo("太阳镜", Color.yellow, "这真是，泰酷辣", "视野受限", RoleId.Sunglasses, false, true);
        public static RoleInfo torch = new RoleInfo("火炬", Color.yellow, "心中有光，照耀四方", "视野增加，无视熄灯", RoleId.Torch, false, true);
        public static RoleInfo multitasker = new RoleInfo("多线程", Color.yellow, "一心多用，[来自和风樱娜的能力注入]", "交互界面透明", RoleId.Multitasker, false, true);
        public static RoleInfo lover = new RoleInfo("恋人", Lovers.color, "是的，我们有一个孩子", "你坠入了爱河", RoleId.Lover, false, true);
        public static RoleInfo mini = new RoleInfo("小孩", Color.yellow, "《未成年人保护法》第127条[不，是第133条]", "没有人能伤害你", RoleId.Mini, false, true);
        public static RoleInfo vip = new RoleInfo("VIP", Color.yellow, "当我去见[和风]的那一天，他们都会知道", "所有人都知道你的死讯", RoleId.Vip, false, true);
        public static RoleInfo indomitable = new RoleInfo("无畏", Color.yellow, "无所畏惧，愈战愈勇", "无法被猜测!", RoleId.Indomitable, false, true);
        public static RoleInfo slueth = new RoleInfo("殡仪员", Color.yellow, "我才是法医！", "报告可知晓死者职业", RoleId.Slueth, false, true);
        public static RoleInfo cursed = new RoleInfo("反骨仔", Color.yellow, "你是船员....至少现在是", "被红狼击杀会变成内鬼", RoleId.Cursed, false, true, true);
        public static RoleInfo invert = new RoleInfo("酒鬼", Color.yellow, "打烊前的最后一杯", "你的移动方向被颠倒了", RoleId.Invert, false, true);
        public static RoleInfo blind = new RoleInfo("胆小鬼", Color.yellow, "天哪！太恐怖了！", "无法报告尸体[除非按R]", RoleId.Blind, false, true);
        public static RoleInfo watcher = new RoleInfo("窥视者", Color.yellow, "窥探一切的阴暗面", "窥探他人投票", RoleId.Watcher, false, true);
        public static RoleInfo radar = new RoleInfo("雷达", Color.yellow, "时刻警惕！", "得知距离最近的玩家位置", RoleId.Radar, false, true);
        public static RoleInfo tunneler = new RoleInfo("管道工程师", Color.yellow, "让我看看这个管道", "完成任务可使用管道", RoleId.Tunneler, false, true);
        public static RoleInfo disperser = new RoleInfo("分散者", Color.red, "大伙！该上路了！", "分散所有人", RoleId.Disperser, false, true);
        public static RoleInfo chameleon = new RoleInfo("变色龙", Color.yellow, "看不见我`看不见我`", "静止时可隐身", RoleId.Chameleon, false, true);
        public static RoleInfo shifter = new RoleInfo("交换师", Color.yellow, "你的能力真好用！", "与船员交换职业,与坏人交换会议结束后死亡", RoleId.Shifter, false, true);
        public static RoleInfo swooper = new RoleInfo("隐身人", Swooper.color, "嘿！你的小可爱突然出现啦", "隐身并杀死敌人", RoleId.Swooper, false, true);
        public static RoleInfo assassin = new RoleInfo("刺客", Color.red, "倒杯水就能猜中了", "艺术就是刺杀！[倒杯水就能猜中了]", RoleId.EvilGuesser, false, true);



        public static List<RoleInfo> allRoleInfos = new List<RoleInfo>() {
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
            //swooper,
            //天启
            juggernaut,
            //末日预言家
            doomsayer,

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

            //badGuesser,
            disperser,
            bloody,
            antiTeleport,
            tiebreaker,
            bait,
            sunglasses,
            torch,
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

        public static List<RoleInfo> getRoleInfoForPlayer(PlayerControl p, bool showModifier = true) {
            List<RoleInfo> infos = new List<RoleInfo>();
            if (p == null) return infos;

            // Modifier
            if (showModifier) {
                // after dead modifier
                if (!CustomOptionHolder.modifiersAreHidden.getBool() || PlayerControl.LocalPlayer.Data.IsDead || AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Ended)
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
                if (Multitasker.multitasker.Any(x => x.PlayerId == p.PlayerId)) infos.Add(multitasker);
                if (p == Mini.mini) infos.Add(mini);
                if (p == Blind.blind) infos.Add(blind);
                if (p == Watcher.watcher) infos.Add(watcher);
                if (p == Radar.radar) infos.Add(radar);
                if (p == Tunneler.tunneler) infos.Add(tunneler);
                if (p == Slueth.slueth) infos.Add(slueth);
                if (p == Swooper.swooper) infos.Add(swooper);
                if (p == Disperser.disperser) infos.Add(disperser);
                if (Invert.invert.Any(x => x.PlayerId == p.PlayerId)) infos.Add(invert);
                if (Chameleon.chameleon.Any(x => x.PlayerId == p.PlayerId)) infos.Add(chameleon);
                if (p == Shifter.shifter) infos.Add(shifter);
                if (Guesser.evilGuesser.Any(x => x.PlayerId == p.PlayerId)) infos.Add(assassin);
            }

            int count = infos.Count;  // Save count after modifiers are added so that the role count can be checked

            // Special roles
            if (p == Jester.jester) infos.Add(jester);
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
            if (p == Jackal.jackal || (Jackal.formerJackals != null && Jackal.formerJackals.Any(x => x.PlayerId == p.PlayerId))) infos.Add(jackal);
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

            // Default roles (just impostor, just crewmate, or hunter / hunted for hide n seek, prop hunt prop ...
            if (infos.Count == count) {
                if (p.Data.Role.IsImpostor)
                    infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek || TORMapOptions.gameMode == CustomGamemodes.PropHunt ? RoleInfo.hunter : RoleInfo.impostor);
                else
                    infos.Add(TORMapOptions.gameMode == CustomGamemodes.HideNSeek ? RoleInfo.hunted : TORMapOptions.gameMode == CustomGamemodes.PropHunt ? RoleInfo.prop : RoleInfo.crewmate);
            }

            return infos;
        }

        public static String GetRolesString(PlayerControl p, bool useColors, bool showModifier = true, bool suppressGhostInfo = false) {
            string roleName;
            roleName = String.Join(" ", getRoleInfoForPlayer(p, showModifier).Select(x => useColors ? Helpers.cs(x.color, x.name) : x.name).ToArray());
            if (Lawyer.target != null && p.PlayerId == Lawyer.target.PlayerId && CachedPlayer.LocalPlayer.PlayerControl != Lawyer.target) 
                roleName += (useColors ? Helpers.cs(Pursuer.color, " §") : " §");
            if (HandleGuesser.isGuesserGm && HandleGuesser.isGuesser(p.PlayerId)) roleName += " (赌怪)";

            if (!suppressGhostInfo && p != null) {
                if (p == Shifter.shifter && (CachedPlayer.LocalPlayer.PlayerControl == Shifter.shifter || Helpers.shouldShowGhostInfo()) && Shifter.futureShift != null)
                    roleName += Helpers.cs(Color.yellow, " ← " + Shifter.futureShift.Data.PlayerName);
                if (p == Vulture.vulture && (CachedPlayer.LocalPlayer.PlayerControl == Vulture.vulture || Helpers.shouldShowGhostInfo()))
                    roleName = roleName + Helpers.cs(Vulture.color, $" ({Vulture.vultureNumberToWin - Vulture.eatenBodies} 剩余)");
                if (Helpers.shouldShowGhostInfo()) {
                    if (Eraser.futureErased.Contains(p))
                        roleName = Helpers.cs(Color.gray, "(被抹除) ") + roleName;
                    if (Doomsayer.playerTargetinformation.Contains(p))
                        roleName = Helpers.cs(Color.gray, "(被预言) ") + roleName;
                    if (Vampire.vampire != null && !Vampire.vampire.Data.IsDead && Vampire.bitten == p && !p.Data.IsDead)
                        roleName = Helpers.cs(Vampire.color, $"(被吸血 {(int)HudManagerStartPatch.vampireKillButton.Timer + 1}) ") + roleName;
                    if (Deputy.handcuffedPlayers.Contains(p.PlayerId))
                        roleName = Helpers.cs(Color.gray, "(被上拷) ") + roleName;
                    if (Deputy.handcuffedKnows.ContainsKey(p.PlayerId))  // Active cuff
                        roleName = Helpers.cs(Deputy.color, "(被上拷) ") + roleName;
                    if (p == Warlock.curseVictim)
                        roleName = Helpers.cs(Warlock.color, "(中术法) ") + roleName;
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
                        roleName = roleName + Helpers.cs(Arsonist.color, $" ({CachedPlayer.AllPlayers.Count(x => { return x.PlayerControl != Arsonist.arsonist && !x.Data.IsDead && !x.Data.Disconnected && !Arsonist.dousedPlayers.Any(y => y.PlayerId == x.PlayerId); })} 剩余)");
                    if (p == Jackal.fakeSidekick)
                        roleName = Helpers.cs(Sidekick.color, $" (fake跟班) ") + roleName;
                        /*
                    if ((p == Swooper.swooper) && Jackal.canSwoop2)
                        roleName = Helpers.cs(Swooper.color, $" (Swooper) ") + roleName;
                        */

                    // Death Reason on Ghosts
                    if (p.Data.IsDead) {
                        string deathReasonString = "";
                        var deadPlayer = GameHistory.deadPlayers.FirstOrDefault(x => x.player.PlayerId == p.PlayerId);

                        Color killerColor = new();
                        if (deadPlayer != null && deadPlayer.killerIfExisting != null) {
                            killerColor = RoleInfo.getRoleInfoForPlayer(deadPlayer.killerIfExisting, false).FirstOrDefault().color;
                        }

                        if (deadPlayer != null) {
                            switch (deadPlayer.deathReason) {
                                case DeadPlayer.CustomDeathReason.Disconnect:
                                    deathReasonString = " - 断开连接";
                                    break;
                                case DeadPlayer.CustomDeathReason.Exile:
                                    deathReasonString = " - 被驱逐 ";
                                    break;
                                case DeadPlayer.CustomDeathReason.Kill:
                                    deathReasonString = $" -被击杀 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Guess:
                                    if (deadPlayer.killerIfExisting.Data.PlayerName == p.Data.PlayerName)
                                        deathReasonString = $" - 猜测失败";
                                    else
                                        deathReasonString = $" - 被猜测而死 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Shift:
                                    deathReasonString = $" - {Helpers.cs(Color.yellow, "职业被交换")} {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.WitchExile:
                                    deathReasonString = $" - {Helpers.cs(Witch.color, "女巫诅咒")}  {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LoverSuicide:
                                    deathReasonString = $" - {Helpers.cs(Lovers.color, "恋人死亡")}";
                                    break;
                                case DeadPlayer.CustomDeathReason.LawyerSuicide:
                                    deathReasonString = $" - {Helpers.cs(Lawyer.color, "客户被投")}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Bomb:
                                    deathReasonString = $" - 被爆炸炸死 {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                                case DeadPlayer.CustomDeathReason.Arson:
                                    deathReasonString = $" - 被焚烧- {Helpers.cs(killerColor, deadPlayer.killerIfExisting.Data.PlayerName)}";
                                    break;
                            }
                            roleName = roleName + deathReasonString;
                        }
                    }
                }
            }
            return roleName;
        }


        static string ReadmePage = "";
        public static async Task loadReadme() {
            if (ReadmePage == "") {
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://raw.githubusercontent.com/TheOtherRolesAU/TheOtherRoles/main/README.md");
                response.EnsureSuccessStatusCode();
                string httpres = await response.Content.ReadAsStringAsync();
                ReadmePage = httpres;
            }
        }
        public static string GetRoleDescription(RoleInfo roleInfo) {
            while (ReadmePage == "") {
            }
                
            int index = ReadmePage.IndexOf($"## {roleInfo.name}");
            int endindex = ReadmePage.Substring(index).IndexOf("### Game Options");
            return ReadmePage.Substring(index, endindex);

        }
    }
}
