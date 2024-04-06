using Refit;

public class TodoistModels
{
    public class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string ParentId { get; set; }
        public int Order { get; set; }
        public int CommentCount { get; set; }
        public bool IsShared { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsInboxProject { get; set; }
        public bool IsTeamInbox { get; set; }
        public string ViewStyle { get; set; }
        public string Url { get; set; }
    }

    public class Task
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public IEnumerable<string> Labels { get; set; }
        public string ProjectId { get; set; }
        public string SectionId { get; set; }
        public string ParentId { get; set; }
        public int Order { get; set; }
        public int Priority { get; set; }
        public string Url { get; set; }
        public int CommentCount { get; set; }
        public string CreatorId { get; set; }
        public string AssigneeId { get; set; }
        public string AssignerId { get; set; }
    }

    public class Section
    {
        public string Id { get; set; }
        public string ProjectId { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
    }

    public class Label
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int Order { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class Comment
    {
        public string Id { get; set; }
        public string TaskId { get; set; }
        public string ProjectId { get; set; }
        public string Content { get; set; }
        public string PostedAt { get; set; }
    }
}

namespace TodoistApi
{
    public interface ITodoistApi
    {
        [Get("/projects")]
        Task<List<TodoistModels.Project>> GetAllProjectsAsync();

        [Post("/projects")]
        Task<TodoistModels.Project> CreateProjectAsync([Body] TodoistModels.Project project);

        [Get("/projects/{projectId}")]
        Task<TodoistModels.Project> GetProjectAsync(string projectId);

        [Post("/projects/{projectId}")]
        Task<TodoistModels.Project> UpdateProjectAsync(string projectId, [Body] TodoistModels.Project project);

        [Delete("/projects/{projectId}")]
        Task DeleteProjectAsync(string projectId);

        [Get("/tasks")]
        Task<List<TodoistModels.Task>> GetActiveTasksAsync();

        [Post("/tasks")]
        Task<TodoistModels.Task> CreateTaskAsync([Body] TodoistModels.Task task);

        [Get("/tasks/{taskId}")]
        Task<TodoistModels.Task> GetActiveTaskAsync(string taskId);

        [Post("/tasks/{taskId}")]
        Task<TodoistModels.Task> UpdateTaskAsync(string taskId, [Body] TodoistModels.Task task);

        [Delete("/tasks/{taskId}")]
        Task DeleteTaskAsync(string taskId);

        [Post("/tasks/{taskId}/close")]
        Task CloseTaskAsync(string taskId);

        [Post("/tasks/{taskId}/reopen")]
        Task ReopenTaskAsync(string taskId);

        [Get("/sections")]
        Task<List<TodoistModels.Section>> GetAllSectionsAsync();

        [Post("/sections")]
        Task<TodoistModels.Section> CreateSectionAsync([Body] TodoistModels.Section section);

        [Get("/sections/{sectionId}")]
        Task<TodoistModels.Section> GetSectionAsync(string sectionId);

        [Post("/sections/{sectionId}")]
        Task<TodoistModels.Section> UpdateSectionAsync(string sectionId, [Body] TodoistModels.Section section);

        [Delete("/sections/{sectionId}")]
        Task DeleteSectionAsync(string sectionId);

        [Get("/labels")]
        Task<List<TodoistModels.Label>> GetAllLabelsAsync();

        [Post("/labels")]
        Task<TodoistModels.Label> CreateLabelAsync([Body] TodoistModels.Label label);

        [Get("/labels/{labelId}")]
        Task<TodoistModels.Label> GetLabelAsync(string labelId);

        [Post("/labels/{labelId}")]
        Task<TodoistModels.Label> UpdateLabelAsync(string labelId, [Body] TodoistModels.Label label);

        [Delete("/labels/{labelId}")]
        Task DeleteLabelAsync(string labelId);

        [Get("/comments")]
        Task<List<TodoistModels.Comment>> GetAllCommentsAsync();

        [Post("/comments")]
        Task<TodoistModels.Comment> CreateCommentAsync([Body] TodoistModels.Comment comment);

        [Get("/comments/{commentId}")]
        Task<TodoistModels.Comment> GetCommentAsync(string commentId);

        [Post("/comments/{commentId}")]
        Task<TodoistModels.Comment> UpdateCommentAsync(string commentId, [Body] TodoistModels.Comment comment);

        [Delete("/comments/{commentId}")]
        Task DeleteCommentAsync(string commentId);
    }
}
