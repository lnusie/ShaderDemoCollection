using UnityEngine;
using UnityEngine.SceneManagement;

namespace CatlikeCoding.NumberFlow.Examples {

	/// <summary>
	/// Automatically switches to another scene two seconds after loading.
	/// Only works if the indicated scene is added in the build settings.
	/// </summary>
	public class MultisceneSwitcher : MonoBehaviour {

		public string nextSceneName;

		void Update () {
			if (Time.timeSinceLevelLoad > 2f) {
				SceneManager.LoadScene(nextSceneName);
			}
		}
	}
}