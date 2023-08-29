using System.Collections;
using UnityEngine;

public class PlayerAttackCol : MonoBehaviour
{
    float duration = 0.1f;
    Collider col;
    public GameObject goFx;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.enabled = false;
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
                hitPoint = GameManager.i._player.AtkPoint;

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

    void OnColEnable(bool show)
    {
        if (col != null)
            col.enabled = show;
    }

    void CreateFX(Collider collider)
    {
        GameObject go = Instantiate(goFx);
        go.transform.position = collider.ClosestPoint(transform.position);
    }
}
