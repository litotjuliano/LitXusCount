interface PlaceholderModuleLayerProps {
  moduleName: string;
}

const PlaceholderModuleLayer = ({ moduleName }: PlaceholderModuleLayerProps) => {
  return (
    <div className='row gy-4'>
      <div className='col-12'>
        <div className='card'>
          <div className='card-body text-center py-80'>
            <h5 className='mb-2'>{moduleName} module</h5>
            <p className='text-muted mb-0'>
              This module is not built yet. It will be implemented as its own OpenSpec change.
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PlaceholderModuleLayer;
