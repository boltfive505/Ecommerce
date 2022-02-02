function get_barcode(text) {
    try {
        var canvas = document.createElement('canvas');
        JsBarcode(canvas, text, {
            displayValue: false,
            margin: 0,
            width: 5,
            height: 300
        });
        return canvas.toDataURL('image/jpeg', 1);
    } catch (err) {
        return '';
    }
}

function get_qrcode(text) {
    /* qrcode-generator.js */
    try {
        var div = document.createElement('div');
        var qr = qrcode(0, 'H');
        qr.addData(text);
        qr.make();
        div.innerHTML=qr.createImgTag(5,0);
        return div.children[0].src;
    } catch (err) {
        return '';
    }

    /* qrcode.min.js */
    // var div = document.createElement('div');
    // new QRCode(div, {
    //     text: text,
    //     width: 300,
    //     height: 300,
    //     orrectLevel : QRCode.CorrectLevel.H
    // });
    // var canvas = div.getElementsByTagName('canvas')[0];
    // return canvas.toDataURL('image/jpeg', 1);
}