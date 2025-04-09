window.createBlobUrl = (byteArray, mimeType) => {
    const bytes = new Uint8Array(byteArray);
    const blob = new Blob([bytes], { type: mimeType });
    return URL.createObjectURL(blob);
};