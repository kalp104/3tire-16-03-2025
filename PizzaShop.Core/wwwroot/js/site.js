let droppingBox = document.querySelector(".droppingBox");
let openeye = document.querySelector(".openeye");
let openeye1 = document.querySelector(".openeye1");
let openeye2 = document.querySelector(".openeye2");
let closeeye = document.querySelector(".closeeye");
let closeeye1 = document.querySelector(".closeeye1");
let closeeye2 = document.querySelector(".closeeye2");
let exampleInputPassword1 = document.querySelector("#exampleInputPassword1");


let count = false;
let count1 = false;
let count2 = false;

function eyeActive() {
    if (count == 1) {
        count = !count;
        console.log("clicked", count);
        openeye.classList.add("active");
        openeye.classList.remove("inactive");
        closeeye.classList.remove("active");
        closeeye.classList.add("inactive");
        exampleInputPassword1.type = "text";
    } else {
        console.log("clicked", count);
        openeye.classList.add("inactive");
        openeye.classList.remove("active");
        closeeye.classList.remove("inactive");
        closeeye.classList.add("active");
        exampleInputPassword1.type = "password";
        count = !count;
    }

}



function eyeActive1() {
    if (count1 == 1) {
        count1 = !count1;
        console.log("clicked", count);
        openeye1.classList.add("active");
        openeye1.classList.remove("inactive");
        closeeye1.classList.remove("active");
        closeeye1.classList.add("inactive");
        exampleInputPassword2.type = "text";
    } else {
        console.log("clicked", count);
        openeye1.classList.add("inactive");
        openeye1.classList.remove("active");
        closeeye1.classList.remove("inactive");
        closeeye1.classList.add("active");
        exampleInputPassword2.type = "password";
        count1 = !count1;
    }

}


function eyeActive2() {
    if (count2 == 1) {
        count2 = !count2;
        console.log("clicked", count2);
        openeye2.classList.add("active");
        openeye2.classList.remove("inactive");
        closeeye2.classList.remove("active");
        closeeye2.classList.add("inactive");
        exampleInputPassword3.type = "text";
    } else {
        console.log("clicked", count2);
        openeye2.classList.add("inactive");
        openeye2.classList.remove("active");
        closeeye2.classList.remove("inactive");
        closeeye2.classList.add("active");
        exampleInputPassword3.type = "password";
        count2 = !count2;
    }

}


