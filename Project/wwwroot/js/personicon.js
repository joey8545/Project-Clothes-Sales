document.addEventListener('DOMContentLoaded', function () {
    const personIcon = document.getElementById('personicon'); // 中文：取得 personicon 元素
    const dropdown = document.getElementById('memberDropdown'); // 中文：取得下拉選單容器

    personIcon.addEventListener('click', function (e) {
        // 中文：點擊個人圖示時
        e.preventDefault();

        // 需要修改：如果下拉選單已顯示，則直接隱藏並結束，不再 fetch
        if (dropdown.style.display === 'block') {
            dropdown.style.display = 'none';
            return; // 中文：直接結束，不再呼叫後端
        }

        // 中文：若尚未顯示下拉選單，才檢查登入並抓取選單
        fetch('/Account/CheckLoginStatus')
            .then(response => response.json())
            .then(data => {
                if (data.isLoggedIn == true) {
                    fetch('/FrontMember/HandleProfileClick')
                        .then(response => response.json())
                        .then(menuData => {
                            if (menuData.success) {
                                dropdown.innerHTML = '';

                                menuData.data.forEach(item => {
                                    const menuItem = document.createElement('a');
                                    menuItem.href = item.url;
                                    menuItem.className = 'dropdown-item py-2';

                                    if (item.text === "登出") {
                                        menuItem.id = 'logoutButton';
                                        menuItem.addEventListener('click', handleLogout);
                                    }

                                    menuItem.innerHTML = `
                                        <div class="d-flex align-items-center" style="gap: 12px;">
                                            <div style="width: 24px; height: 24px;" class="d-flex align-items-center justify-content-center">
                                                <i class="bi bi-${getIconForMenuItem(item.text)}"></i>
                                            </div>
                                            <span>${item.text}</span>
                                        </div>
                                    `;
                                    dropdown.appendChild(menuItem);
                                });

                                dropdown.style.display = 'block';
                            }
                        })
                        .catch(error => {
                            console.error('Error fetching menu:', error);
                        });
                } else {
                    window.location.href = '/FrontMember/fcreate';
                }
            })
            .catch(error => {
                console.error('Error checking login status:', error);
                window.location.href = '/FrontMember/fcreate';
            });
    });

    function getIconForMenuItem(text) {
        switch (text) {
            case '個人資料': return 'person-circle';
            case '我的訂單': return 'bag';
            case '優惠券': return 'ticket-perforated';
            case '後檯': return 'unity';
            case '登出': return 'box-arrow-right';
            default: return 'chevron-right';
        }
    }

    function handleLogout(e) {
        e.preventDefault();
        fetch('/Account/Logout', { method: 'POST' })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    window.location.href = '/FrontHome/FrontIndex';
                } else {
                    console.error('Logout failed:', data.message);
                }
            })
            .catch(error => {
                console.error('Error during logout:', error);
            });
    }

    // 點擊外部關閉下拉選單
    document.addEventListener('click', function (event) {
        if (dropdown.style.display === 'block' &&
            !dropdown.contains(event.target) &&
            !personIcon.contains(event.target)) {
            dropdown.style.display = 'none';
        }
    });
});
