function clear(id) {
    document.getElementById(id).innerHTML = '';
}

function appendChild(id, value)
{
    var el = document.createElement("span");
    el.innerHTML = value;
    el.style.display = 'block';
    document.getElementById(id).appendChild(el);
}

function appendSpanChild(object, value) {
    var el = document.createElement("span");
    el.innerHTML = value;
    el.style.display = 'block';
    object.appendChild(el);
}

function appendInputTextChild(object, id, value) {
    var input = document.createElement("input");
    input.type = "text";
    input.id = id;
    input.value = value;
    object.appendChild(input);

    input.addEventListener("keyup", function (event) {
        event.preventDefault();
        debugger
        if (event.keyCode == 13) {
            //alert(this);
            var parant = findAncestor(this, "student");
            var obj = createJson(parant);
            //alert(parant);

            studentSystem.OnSend(obj);
        }
    });
}

function appendInputHiddenChild(object, id, value) {
    var input = document.createElement("input");
    input.type = "hidden";
    input.id = id;
    input.value = value;
    object.appendChild(input);
}

function findAncestor(el, cls) {
    while ((el = el.parentElement) && !el.classList.contains(cls));
    return el;
}

function createJson(parant) {
    var firstName = parant.querySelector("#first-name").value;
    var lastName = parant.querySelector("#last-name").value;
    var age = parant.querySelector("#age").value;
    var id = parant.querySelector("#id").value;

    var cours = null;
    var arr = [];
    var index = -1;
    do {
        index++;
        var cours = parant.querySelector(".courses").querySelector("#course" + index);

        if (cours) {
            arr.push({
                id: cours.querySelector("#cource-id").value,
                name: cours.querySelector("#cource-name").value,
                Description: cours.querySelector("#cource-description").value,
                Evaluation: cours.querySelector("#cource-evaluation").value
            });
        }

    } while (cours);
    
    //for (i = 0; i < cours.length; i++) {
    //    arr.push({
    //        name: cours[i].querySelector("#cource-name").value,
    //        Description: cours[i].querySelector("#cource-description").value,
    //        Evaluation: cours[i].querySelector("#cource-evaluation").value
    //    });
    //}

    var student = {
        id: id,
        firstName: firstName,
        lastName: lastName,
        age: age,
        recCourses: arr
    };

    return JSON.stringify(student);
}