import { useState } from 'react';
import Button from 'react-bootstrap/Button';
import Modal from 'react-bootstrap/Modal';

function VerifyModal({ show, modalTitle, modalBody, handleClose, onVerify }) {

  return (
    <>
      <Modal show={show} onClick={handleClose}>
        <Modal.Header closeButton>
          <Modal.Title>{modalTitle}</Modal.Title>
        </Modal.Header>
        <Modal.Body>{modalBody}</Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleClose} >
            Close
          </Button>
          <Button variant="primary" onClick={onVerify}>
            Yes
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
}

export default VerifyModal;