document.getElementById('startButton').addEventListener('click', function() {
    document.querySelectorAll('.container:not(#unityContainer), footer, #home').forEach(function(el) {
        el.style.display = 'none';
    });
    document.getElementById('unityContainer').style.display = 'block';
    document.getElementById('unityFrame').src = 'unity/index.html';
});

document.getElementById('backButton').addEventListener('click', function() {
    document.getElementById('unityContainer').style.display = 'none';
    document.querySelectorAll('.container:not(#unityContainer), footer, #home').forEach(function(el) {
        el.style.display = 'block';
    });
    document.getElementById('unityFrame').src = '';
});

document.querySelectorAll('.navbar-nav a').forEach(anchor => {
    anchor.addEventListener('click', function(e) {
        e.preventDefault();
        if (document.getElementById('unityContainer').style.display === 'block') {
            document.getElementById('unityContainer').style.display = 'none';
            document.querySelectorAll('.container:not(#unityContainer), footer, #home').forEach(function(el) {
                el.style.display = 'block';
            });
            document.getElementById('unityFrame').src = '';
        }
        const target = document.querySelector(this.getAttribute('href'));
        if (target) {
            target.scrollIntoView({ behavior: 'smooth' });
        }
    });
});

// 侧边栏“靠近再出来”功能
const sidebar = document.getElementById('sidebar');
document.addEventListener('mousemove', function(e) {
    if (e.clientX < 50) { // 鼠标靠近左侧50像素范围内时
        sidebar.style.left = '0';
    } else if (e.clientX > 200) { // 鼠标离开侧边栏区域时
        sidebar.style.left = '-200px';
    }
});