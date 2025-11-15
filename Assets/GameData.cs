using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public static GameData Instance;
    
    // Variable nyawa yang akan bertahan saat scene di-reload.
    public int currentLives = 3;
    public int maxLives = 3; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // BARIS KRUSIAL: Objek ini akan bertahan antar scene.
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Hancurkan duplikat jika objek ini sudah ada.
            Destroy(gameObject); 
        }
    }
    
    // Fungsi untuk mereset nyawa saat Game Over total (nyawa dari 0 kembali ke max)
    public void ResetLives()
    {
        currentLives = maxLives;
    }
}