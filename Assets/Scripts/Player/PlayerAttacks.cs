using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerAttacks : MonoBehaviour
{
    public Volume vol;
    Vignette vig;
    public CinemachineFreeLook cm;
    CharacterController3D player;
    public Collider hitter;
    public float maxCount = 0.1f, parryWindow = 1f;
    public float count = 0;
    bool lastPress;
    public float slowMoMax;
    float slowMoTimer;
    float parryTimer;
    bool parryTriggered;
    Animator anim;
    InputMaster input;
    void Start()
    {
        vol.profile.TryGet(out vig);
        player = GetComponent<CharacterController3D>();
        anim = GetComponent<Animator>();
    }
    void OnEnable()
    {
        input = new InputMaster();
        input.Enable();
    }
    void OnDisable()
    {
        input.Disable();
    }
    public void PlayerAttacked(EnemyDamageCollider other)
    {
        if(input.Player.Block.controls.Any(c => c.IsPressed()))
        {
            if (parryTimer > 0)
            {
                slowMoTimer = slowMoMax;
                anim.SetBool("Parry", true);
                other.transform.parent.GetComponent<Enemy>().Staggered = true;
                other.transform.parent.GetComponent<Enemy>().ColliderOff();
            }
            else if (player.stamina > 0)
            {
                player.stamina -= 10;
                player.staminaCount = player.staminaRecoveryTime;
            }
            else
            {
                GetComponent<Health>().Damage(other.damage, other.GetComponent<Collider>(), other.knockback);
            }
        }
        else
        {
            GetComponent<Health>().Damage(other.damage, other.GetComponent<Collider>(), other.knockback);
        }
        
    }
    private void FixedUpdate()
    {
        if (slowMoTimer >= 0)
        {
            //vig.color = new ColorParameter(new Color(0, 0, 255, 1), true);
            //vig.intensity = new ClampedFloatParameter(Mathf.Lerp((float)vig.intensity, 0.6f, 0.1f), 0, 1, true);
            cm.m_Lens.FieldOfView = Mathf.Lerp(cm.m_Lens.FieldOfView, 30, 0.1f);
            Time.timeScale = 0.1f;
            slowMoTimer -= Time.unscaledDeltaTime;
        }
        else
        {
            //vig.color = new ColorParameter(new Color(0, 0, Mathf.Lerp(((Color)vig.color).b, 0, 0.1f), 1), true);
            //vig.intensity = new ClampedFloatParameter(Mathf.Lerp((float)vig.intensity, 0.45f, 0.1f), 0, 1, true);
            cm.m_Lens.FieldOfView = Mathf.Lerp(cm.m_Lens.FieldOfView, 40, 0.1f);
            Time.timeScale = 1f;
        }
    }
    void Update()
    {
        
        if (parryTimer >= 0) parryTimer -= Time.deltaTime;
        if (input.Player.Sprint.controls.Any(c => c.IsPressed())) return;
        if (input.Player.Block.controls.Any(c => c.IsPressed())) 
        { 
            anim.SetInteger("Attack", 0);
            anim.SetBool("Block", true);
            if (!parryTriggered) parryTimer = parryWindow;
            parryTriggered = true;
            return;
        }
        else
        {
            if (parryTimer <= 0) parryTriggered = false;
            anim.SetBool("Block", false);
        }
        if (!lastPress && input.Player.Attack.controls.Any(c => c.IsPressed()))
        {
            anim.SetInteger("Attack", anim.GetInteger("Attack") + 1);
        }
        lastPress = input.Player.Attack.controls.Any(c => c.IsPressed());
    }
    void ComboDone(int val)
    {
        if(anim.GetInteger("Attack") <= val || val >= 3) anim.SetInteger("Attack", 0);
    }
    void Vel()
    {
        GetComponent<CharacterController3D>().speed += .2f;
        GetComponent<CharacterController3D>().SpeedCalc(transform.forward, 1);
    }
    void ColliderOn()
    {
        player.stamina -= 10;
        player.staminaCount = player.staminaRecoveryTime;
        hitter.enabled = true;
    }
    void ColliderOff()
    {
        hitter.enabled = false;
    }
    void ParryDone()
    {
        anim.SetBool("Parry", false);
    }
}
