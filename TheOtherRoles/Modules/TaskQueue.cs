using System.Threading.Tasks;

namespace TheOtherRoles.Modules;

public class TaskQueue
{
#nullable enable
    public static TaskQueue? MainQueue { get; private set; }
#nullable disable
    public static TaskQueue GetOrCreate()
    {
        if (MainQueue != null)
            return MainQueue;

        return MainQueue = new TaskQueue();
    }


    public Task CurrentTask;
    public readonly Queue<Task> Tasks = [];

    public bool TaskStarting;

    private readonly Dictionary<Task, Action> TaskOnCompleted = new();
    public TaskQueue StartTask(Action action, string Id, Action OnCompleted = null)
    {
        var task = new Task(() =>
        {
            Info($"Start TaskQueue Id:{Id}");
            try
            {
                action();
            }
            catch (Exception e)
            {
                Error(e);
                Error($"加载失败 TaskQueue Id:{Id}");
            }
        });
        if (OnCompleted != null)
            TaskOnCompleted[task] = OnCompleted;
        Tasks.Enqueue(task);

        if (!TaskStarting)
            StartNew();
        return this;
    }

    public void StartNew()
    {
        if (Tasks.Count == 0 || TaskStarting) return;
        TaskStarting = true;
        Task.Run(() =>
            {
                Start();
                TaskStarting = false;
            }
        );
        return;

        void Start()
        {
            if (Tasks.Count == 0) return;
            CurrentTask = Tasks.Dequeue();
            CurrentTask.Start();
            CurrentTask.GetAwaiter().OnCompleted(() =>
            {
                if (TaskOnCompleted.TryGetValue(CurrentTask, out var ac))
                {
                    ac();
                    TaskOnCompleted.Remove(CurrentTask);
                }
                Start();
            });
        }
    }
}