using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    [SerializeField] private BaseCharacterController m_characterOwner;

    //public void Animation_LeapCharacter(object leapParameters)
    //{
    //    m_characterOwner.LeapForward(new Vector2(leapX, leapY));
    //}

    public void Animation_StopCharacter()
    {
        m_characterOwner.SetTargetVelocityX(0f);
    }
}
