namespace MakeItWork
{
    internal static class SkipTutorialPatch
    {
        internal static void QuestGoalInitPost(ref QuestGoal __instance)
        {
            __instance.ForceComplete();
            __instance.Complete();
            __instance.quest._isCompleted = true;
            __instance.quest.OnQuestCompleted();
        }
    }
}
