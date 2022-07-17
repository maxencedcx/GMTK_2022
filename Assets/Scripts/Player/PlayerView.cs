using UnityEngine;

public class PlayerView : MonoBehaviour
{
    public RSLib.Audio.ClipProvider FootstepsClip;

    public void OnFootstep()
    {
        RSLib.Audio.AudioManager.PlaySound(this.FootstepsClip);
    }
}
