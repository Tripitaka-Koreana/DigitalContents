using System.Collections.Generic;
using UnityEngine;

// �÷��̾� ���� ������ �����ϰ�, �ش� ���� ���� ������Ʈ ����� �����ϴ� ��ũ��Ʈ
public class cshAttackArea : MonoBehaviour
{
    // ���� ���� ���� ���� �ִ� Collider ����Ʈ�� �ܺο��� ��ȸ�� �� �ֵ��� �Ӽ����� ����
    public List<Collider> colliders
    {
        get
        {
            if (0 < colliderList.Count)
            {
                // colliderList �ȿ� null(�ı��� ������Ʈ) ���� ������ �����ϰ� ��ȯ
                colliderList.RemoveAll(c => c == null);
            }
            return colliderList;
        }
    }

    // ������ ���ο��� ����ϴ� �浹ü ����Ʈ
    private List<Collider> colliderList = new List<Collider>();

    // ���� ����(Trigger)�� ������Ʈ�� ������ �� ȣ���
    private void OnTriggerEnter(Collider other)
    {
        // �±װ� 'BreakableObject' �Ǵ� 'Enemy'�� ��츸 ����Ʈ�� �߰�
        if (other.CompareTag("BreakableObject") || other.CompareTag("Enemy"))
        {
            colliders.Add(other); // ���� �Ӽ��� ���������� colliderList�� ��ȯ�ϹǷ� ��� ����
        }
    }

    // ���� �������� ������Ʈ�� ������ �� ȣ���
    private void OnTriggerExit(Collider other)
    {
        // ���� ������Ʈ�� ����̸� ����Ʈ���� ����
        if (other.CompareTag("BreakableObject") || other.CompareTag("Enemy"))
        {
            colliders.Remove(other);
        }
    }
}
