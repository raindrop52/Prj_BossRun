using System.Collections;
using UnityEngine;

public class PlayerAttackCol : MonoBehaviour
{
    public float duration = 0.1f;
    Collider col;
    public GameObject goFx;
    public GameObject groundHitFx;
    public TrailRenderer trailFx;

    private void Awake()
    {
        col = GetComponent<Collider>();
        OnColEnable(false);
    }

    public void Show()
    {
        StartCoroutine(AutoDisable());
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enemy �±׿� �浹�� ���
        if (other.CompareTag("Enemy"))
        {
            CreateFX(other);

            IDamage damage = other.GetComponent<IDamage>();
            
            int hitPoint = 0;
            if (GameManager.i != null)
            {
                // �÷��̾��� ���ݷ� ������
                hitPoint = GameManager.i._player.AtkPoint;
                // ��ų ������ ����
                GameManager.i._player.CreateSKGague(other.transform);
            }

            if (damage != null)
                damage.Damage(hitPoint);
        }
    }

    IEnumerator AutoDisable()
    {
        OnColEnable(true);

        // duration �Ŀ� ������Ʈ�� ��������� ��
        yield return new WaitForSeconds(duration);

        OnColEnable(false);
        //gameObject.SetActive(false);
    }

    public void OnColEnable(bool show)
    {
        if (col != null)
            col.enabled = show;

        ShowTrail(show);
    }

    void ShowTrail(bool show)
    {
        if (trailFx != null)
            trailFx.enabled = show;
    }

    void CreateFX(Collider collider)
    {
        GameObject go = Instantiate(goFx);
        go.transform.position = collider.ClosestPoint(transform.position);
    }
}
