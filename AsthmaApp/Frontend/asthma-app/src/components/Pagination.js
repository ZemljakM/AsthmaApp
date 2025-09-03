import React from 'react';
import ReactPaginate from 'react-paginate';
import '../css_files/Pagination.css';

const Pagination = ({ pageCount, onPageChange }) => {
    return (
        <div className="pagination-container">
            <ReactPaginate
                previousLabel={'←'}
                nextLabel={'→'}
                breakLabel={'...'}
                breakClassName={'break-me'}
                pageCount={pageCount}
                marginPagesDisplayed={2}
                pageRangeDisplayed={5}
                onPageChange={onPageChange}
                containerClassName={'pagination'}
                activeClassName={'active'}
                pageClassName={'page-item'}
                previousClassName={'page-item'}
                nextClassName={'page-item'}
                disabledClassName={'disabled'}
            />
        </div>
    );
};

export default Pagination;