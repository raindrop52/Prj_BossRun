using System.Collections;
using UnityEngine;

public class PlayerAttackCol : MonoBehaviour
{
    float duration = 0.1f;

    private void OnEnable()
    {
        StartCoroutine(AutoDisable());
    }

    private void OnTriggerEnter(Collider other)
    {
        // Enemy �±׿� �浹�� ���
        if (other.CompareTag("Enemy"))
        {
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
        // duration �Ŀ� ������Ʈ�� ��������� ��
        yield return new WaitForSeconds(duration);

        gameObject.SetActive(false);
    }
}
