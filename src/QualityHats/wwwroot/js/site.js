function imagePreview(elem) {
    /* get file type */
    const fileType = elem.files[0].type;
    /* create file */
    image = document.getElementById('ProductImage')
    if (null == document.getElementById('ProductImage')) {
        image = document.createElement('img');
    }
    
    /* instance FileReader */
    reader = new FileReader();
    /* error handle */
    if (/^image\//.test(fileType) === false) return;
    /* show image */
    reader.onload = function (e) {
        /* add path  */
        image.src = e.target.result;
        if (null == document.getElementById('ProductImage')) {
            /* insert to element */
            elem.parentNode.appendChild(image);
        }
    }
    /* read file */
    reader.readAsDataURL(elem.files[0]);
}