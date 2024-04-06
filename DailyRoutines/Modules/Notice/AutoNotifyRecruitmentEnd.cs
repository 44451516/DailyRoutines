using System;
using DailyRoutines.Infos;
using DailyRoutines.Managers;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;

namespace DailyRoutines.Modules;

[ModuleDescription("AutoNotifyRecruitmentEndTitle", "AutoNotifyRecruitmentEndDescription", ModuleCategories.Notice)]
public class AutoNotifyRecruitmentEnd : DailyModuleBase
{
    public override void Init()
    {
        Service.Chat.ChatMessage += OnChatMessage;
    }

    private static void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
    {
        if (type != XivChatType.SystemMessage) return;
        if (Service.Condition[ConditionFlag.BoundByDuty] || Service.Condition[ConditionFlag.BoundByDuty56] || Service.Condition[ConditionFlag.BoundByDuty95] || Service.Condition[ConditionFlag.BoundToDuty97]) return;

        var content = message.ExtractText();

        if (!content.StartsWith("招募队员结束") && !content.Contains("Party recruitment ended") && !content.Contains("パーティ募集の人数を満たしたため終了します。")) return;

        var title = "";
        if (content.StartsWith("招募队员结束"))
        {
            var parts = content.Split(["，"], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                Service.Notice.Notify(parts[0], parts[1].Trim('。'));
                return;
            }

            title = parts[0].Trim('。');
        }

        if (content.Contains("Party recruitment ended"))
        {
            var parts = content.Split(["."], StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                Service.Notice.Notify(parts[1], parts[0]);
                return;
            }

            title = parts[0];
        }

        if (content.Contains("パーティ募集の人数を満たしたため終了します。"))
        {
            title = content;
        }

        Service.Notice.Notify(title, title);
    }

    public override void Uninit()
    {
        Service.Chat.ChatMessage -= OnChatMessage;

        base.Uninit();
    }
}
