//"use strict"
//const lists = document.querySelectorAll(".list");
//let storageType = document.querySelector(".links__dropdown li a");
//console.log(storageType.innerText);

//lists.forEach(list => {
//    list.addEventListener("click", function (e) {
//        if (e.target.closest(".item-list__btn-complete")) {
//            SendCompleteStatus(e.target);
//        }
//        if (e.target.closest(".item-list__btn-ed")) {
//            UpdateTask(e);
//        }
       
//    });
//});

//function SendCompleteStatus(el) {
//    const id = el.dataset.id;
//    const isCompleted = el.checked; 
    
//    if (storageType.innerText === "MySql") {
//        window.location.href = `/Home/MarkComplete?Id=${id}&IsCompleted=${isCompleted}`;
//    } else  {
//        window.location.href = `/XML/MarkComplete?Id=${id}&IsCompleted=${isCompleted}`;
//    }
       
//}

////function UpdateTask(e) {
////    const item = e.target.closest(".item-list");
////    const id = item.querySelector(".item-list__btn-complete").dataset.id;
////    const time = item.querySelector(".item-list__additional span").textContent || '';
   
////    const category = item.querySelector(".item-list__category").dataset.category || '';
////    console.info(category);
////    const textEl = item.querySelector(".item-list__text");
////    const oldText = textEl.textContent;

////    const textarea = document.createElement("textarea");
////    textarea.classList.add("edit-textarea");
////    textarea.value = oldText;

////    const dataInput = document.createElement("input");
////    dataInput.type = 'datetime-local';


////    const saveBtn = document.createElement("button");
////    saveBtn.textContent = "Save";
////    saveBtn.classList.add("save-btn");

////    textEl.replaceWith(textarea);
////    item.querySelector(".item-list__body").appendChild(saveBtn);

////    saveBtn.addEventListener("click", function () {

////        const updateForm = document.createElement("form");
////        updateForm.method = 'POST';
////        updateForm.className = 'hidden-form';
////        updateForm.action = 'Home/Update';

////        const updateTextArea = document.createElement("textarea");
////        updateTextArea.value = textarea.value;
////        updateTextArea.name = 'Text';

////        const idElForm = document.createElement("input");
////        idElForm.type = "hidden";
////        idElForm.name = 'Id';
////        idElForm.value = id;

////        updateForm.appendChild(updateTextArea);
////        updateForm.appendChild(idElForm);
////        document.body.appendChild(updateForm);
////        updateForm.submit();

////        textarea.remove();
////        saveBtn.remove();

////    });
////}


