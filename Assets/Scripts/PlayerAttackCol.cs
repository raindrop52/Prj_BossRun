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
        // Enemy 태그에 충돌한 경우
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
        // duration 후에 오브젝트가 사라지도록 함
        yield return new WaitForSeconds(duration);

        gameObject.SetActive(false);
    }
}
