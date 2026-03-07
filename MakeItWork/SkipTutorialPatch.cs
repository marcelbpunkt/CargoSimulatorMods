namespace MakeItWork
{
    internal static class SkipTutorialPatch
    {
        internal static void QuestGoal_Init_Post(ref QuestGoal __instance)
        {
            __instance.ForceComplete();
            __instance.Complete();
            __instance.quest._isCompleted = true;
            __instance.quest.OnQuestCompleted();
        }
    }
}
