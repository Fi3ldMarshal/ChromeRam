using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Chrome : MonoBehaviour {

    public float moveSpeedMax = 5f;

    public float rotateSpeed = 2f;

    public KeyCode keyForward = KeyCode.W, keyBack = KeyCode.S, keyLeft = KeyCode.A, keyRight = KeyCode.D;

    float size_ = 1f;

    public float sizeChangeTime = .25f;

    public float size { get { return size_; } set { size_ = value; } }

    public float acc = .5f, decc = .25f;

    Rigidbody2D phy;

    Collider2D coll;

    public static Chrome main;

    void Awake()
    {
        phy = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        main = this;
    }

	void Start () {
        
    }

    void Die()
    {
        transform.localScale = Vector3.zero;
        print("Dead");
        phy.velocity = Vector3.zero;
        coll.enabled = false;
        this.enabled = false;
    }

    IEnumerator ChangeSizeT(float t, float s)
    {
        print("ch from " + gameObject.name);
        yield return new WaitForSeconds(t);
        size_ = s;
        yield break;
    }

    void ChangeSize(float gain)
    {
        size += gain;
        if (size <= 0f)
            Die();
        //transform.localScale = Vector3.one * size;
    }

    Vector2 vel = Vector2.zero, axis = Vector2.zero;

    float l = 0f;

	void Update () {
        if (axis == Vector2.zero)
        {
            axis = new Vector2(Input.GetKey(keyLeft) ? -1f : Input.GetKey(keyRight) ? 1f : 0f,
                               Input.GetKey(keyForward) ? 1f : Input.GetKey(keyBack) ? -1f : 0f);
            axis.Normalize();
        }
	}

    void FixedUpdate()
    {
        if (axis != Vector2.zero)
        {
            vel += axis * Time.fixedDeltaTime * acc;
        }
        else
        {
            if (vel != Vector2.zero)
            {
                Vector2 delta = -(vel.normalized) * decc * Time.fixedDeltaTime;
                vel = delta.normalized == (vel + delta).normalized ? Vector2.zero : vel + delta;
            }
        }
        //vel += -(vel.normalized) * decc * Time.deltaTime;
        vel = Vector2.ClampMagnitude(vel, moveSpeedMax);
        //print(vel);
        phy.velocity = vel;
        transform.Rotate(0, 0, rotateSpeed);
        if (size != transform.localScale.x)
        {
            if (l == 0f)
                l = Mathf.Abs(size - transform.localScale.x);
            float s = Mathf.MoveTowards(transform.localScale.x, size, l / sizeChangeTime * Time.fixedDeltaTime);
            transform.localScale = Vector3.one * s;
        }
        else l = 0f;
        axis = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == 8)
        {
            Item it = col.GetComponent<Item>();
            if(it)
            {
                ChangeSize(it.sizeGain);
            }
            col.gameObject.SetActive(false);
        }
    }

}
