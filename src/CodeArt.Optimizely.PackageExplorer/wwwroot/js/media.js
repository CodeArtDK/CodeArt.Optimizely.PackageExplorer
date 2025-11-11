window.createBlobUrl = (byteArray, mimeType) => {
    const bytes = new Uint8Array(byteArray);
    const blob = new Blob([bytes], { type: mimeType });
    return URL.createObjectURL(blob);
};

window.downloadFileFromStream = async (fileName, streamReference) => {
    const arrayBuffer = await streamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
};