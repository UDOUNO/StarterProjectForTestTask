using UnityEngine.SceneManagement;

public class SceneLoader
{
    private readonly EventManager _eventManager;

    public SceneLoader(EventManager eventManager)
    {
        _eventManager = eventManager;

        _eventManager.Subscribe<EventsProvider.PlayerFellEvent>(OnPlayerFell);
    }

    private void OnPlayerFell(EventsProvider.PlayerFellEvent eventData)
    {
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int sceneCount = SceneManager.sceneCountInBuildSettings;

        if (sceneCount <= 1)
            return;

        int nextSceneIndex = (currentSceneIndex + 1) % sceneCount;

        SceneManager.LoadScene(
            nextSceneIndex,
            LoadSceneMode.Single
        );
    }

    public void Dispose()
    {
        _eventManager.Unsubscribe<EventsProvider.PlayerFellEvent>(OnPlayerFell);
    }
}