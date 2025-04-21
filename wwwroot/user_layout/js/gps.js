	document.querySelectorAll(".sidebox-title").forEach((item)=>{item.addEventListener("click", function () 
    {
    const content = this.closest(".group-sidebox").querySelector(".sidebox-content");
    content.classList.toggle("active");
	})
});

document.querySelectorAll('.toggle-icon').forEach((icon) => {
    icon.addEventListener('click', function (e) {
        e.preventDefault(); 
        const submenu = this.parentNode.nextElementSibling; 
        
        submenu.classList.toggle('active');
     
        if(submenu.classList.contains('active'))
        {
            this.textContent  = '-';
        }
        else
        {
            this.textContent  = '+';
        }
    });
});
